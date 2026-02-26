// ...existing code...
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Summarizer.Services;

internal class TextEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>, IDisposable
{
    private readonly MLContext _machineLearningContext = new(seed: 42);
    private readonly object _modelLock = new();
    private ITransformer? _trainedTransformer;

    private class EmbeddingInput
    {
        public string Text { get; set; } = string.Empty;
    }

    private class EmbeddingOutput
    {
        [VectorType]
        public VBuffer<float> Features { get; set; }
    }

    public void TrainOnCorpusTexts(IEnumerable<string> corpusTexts)
    {
        IDataView trainingData = _machineLearningContext.Data.LoadFromEnumerable(
            corpusTexts.Select(text => new EmbeddingInput { Text = text ?? string.Empty }));

        IEstimator<ITransformer> textFeaturizationPipeline = _machineLearningContext.Transforms.Text.FeaturizeText(
            outputColumnName: nameof(EmbeddingOutput.Features),
            inputColumnName: nameof(EmbeddingInput.Text));

        lock (_modelLock)
        {
            _trainedTransformer = textFeaturizationPipeline.Fit(trainingData);
        }
    }

    private IReadOnlyList<Embedding<float>> GenerateEmbeddingsFromTexts(IList<string> texts, CancellationToken cancellationToken)
    {
        ITransformer trainedSnapshot;
        lock (_modelLock)
        {
            trainedSnapshot = _trainedTransformer ?? throw new InvalidOperationException("Embedding model is not trained. Call TrainOnCorpusTexts(corpusTexts) first.");
        }

        IDataView inputDataView = _machineLearningContext.Data.LoadFromEnumerable(
            texts.Select(text => new EmbeddingInput { Text = text ?? string.Empty }));

        IDataView transformedDataView = trainedSnapshot.Transform(inputDataView);

        List<Embedding<float>> embeddingList = new List<Embedding<float>>(texts.Count);

        foreach (EmbeddingOutput outputRow in _machineLearningContext.Data.CreateEnumerable<EmbeddingOutput>(transformedDataView, reuseRowObject: false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            float[] denseVector = outputRow.Features.DenseValues().ToArray();
            NormalizeToUnitLengthInPlace(denseVector);

            embeddingList.Add(new Embedding<float>(denseVector));
        }

        return embeddingList;
    }

    async Task<GeneratedEmbeddings<Embedding<float>>> IEmbeddingGenerator<string, Embedding<float>>.GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options,
        CancellationToken cancellationToken)
    {
        IList<string> texts = values as IList<string> ?? values.ToList();

        IReadOnlyList<Embedding<float>> embeddings = await Task.Run(
            () => GenerateEmbeddingsFromTexts(texts, cancellationToken),
            cancellationToken);

        return new GeneratedEmbeddings<Embedding<float>>(embeddings);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceType == typeof(IEmbeddingGenerator<string, Embedding<float>>)) return this;
        return null;
    }

    private static void NormalizeToUnitLengthInPlace(float[] vector)
    {
        double sumOfSquares = 0;
        for (int index = 0; index < vector.Length; index++) sumOfSquares += vector[index] * vector[index];

        double length = Math.Sqrt(sumOfSquares);
        if (length <= 0) return;

        float inverseLength = (float)(1.0 / length);
        for (int index = 0; index < vector.Length; index++) vector[index] *= inverseLength;
    }

    public void Dispose()
    {
        lock (_modelLock)
        {
            _trainedTransformer = null;
        }
    }
}