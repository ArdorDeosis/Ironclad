# Overview

This package provides a noise generation library with implementations for 4-byte and 8-byte values. It includes deterministic discrete noise fields for 1D to 9D inputs. The library is based on algorithms introduced by [Squirrel Eiserloh](#reference).

# Features

## 4 or 8 Bytes

The library offers both [4-byte](Noise4Byte.cs) and [8-byte](Noise8Byte.cs) implementations.

|        | integer | floating point |        implementation         |
|:------:|:-------:|:--------------:|:-----------------------------:|
| 4 Byte | `uint`  |    `float`     | [`Noise4Byte`](Noise4Byte.cs) |
| 8 Byte | `ulong` |    `double`    | [`Noise8Byte`](Noise8Byte.cs) |

This allows users to choose between performance and precision based on their specific needs.

## Up to 9 Dimensions

The library supports noise generation for up to 9 dimensions, providing flexibility for various applications.
This range covers most common use cases in game development, procedural generation, and other fields requiring noise.

## Interfaces for Every Combination

The library includes interfaces for every combination of 4 or 8 byte values and 1-9 dimensions:
- This comprehensive set of interfaces allows for better substitution and testing.
- Users can easily switch between different implementations or create mock objects for testing purposes.

(see [Interfaces directory](Interfaces))

# Reference

The noise algorithm used in this library was introduced by Squirrel Eiserloh. You can watch his talk on YouTube: [Squirrel Eiserloh's GDC 2017 Talk](https://www.youtube.com/watch?v=LWFzPP8ZbdU).