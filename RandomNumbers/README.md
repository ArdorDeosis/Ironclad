# Overview

This package provides a versatile random number generation system built on a noise generator with implementations for 4-byte and 8-byte values. This module allows for deterministic generation of random numbers with a state counter, making it ideal for applications requiring reproducibility and sequence tracking.

# Features

## 4 or 8 Byte Implementations

The library supports both 4-byte and 8-byte implementations, giving users the flexibility to choose between performance and precision:

|        | integer | floating point |                                    implementation                                    |
|:------:|:-------:|:--------------:|:------------------------------------------------------------------------------------:|
| 4 Byte | `uint`  |    `float`     |      [`RandomNumberGenerator`](Ironclad.RandomNumbers/RandomNumberGenerator.cs)      |
| 8 Byte | `ulong` |    `double`    | [`RandomNumberGenerator8Byte`](Ironclad.RandomNumbers/RandomNumberGenerator8Byte.cs) |

## Stateful Random Number Generation

The random number generator maintains a state counter, which is incremented with each generated number. This allows tracking of the sequence and easy reproduction of results.

# Reference

This random number generation module is inspired by a talk by Squirrel Eiserloh. You can watch his talk on YouTube: [Squirrel Eiserloh's GDC 2017 Talk](https://www.youtube.com/watch?v=LWFzPP8ZbdU).