# Property synchronization example

This repository contains an example implementation of property synchronization between two PDF viewers. The purpose of this example is to demonstrate how to keep the properties of two PDF viewers in sync, ensuring that actions performed on one viewer are reflected in the other.

## Implementation Details

The property synchronization is achieved by establishing a communication between the two PDF viewer instances. The following steps are involved:

1. When a property is changed in one PDF viewer, an event named <b>PropertyChanged</b> is triggered.
2. The property changes information are available in the event arguments.
3. The receiving viewer updates its properties based on the received information, resulting in synchronized behavior.

## What are the properties covered in this example?

The following property are covered in this example.
1. Zoom.
2. Scroll offsets.