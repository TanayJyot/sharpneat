// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.BlackBox;

/// <summary>
/// Represents an abstract 'black box' function, with and input vector, an Activate() method that takes
/// the inputs to produce an output vector. I.e. 'black box' here could also be described as a multivariate
/// mathematical function.
///
/// Typically a black box will be a neural network, whereby we set the input vector, activate the network, and
/// read its output vector. However, in principle a black box could be any kind of function or information processing
/// system such as a C# program or a genetic programming tree.
/// </summary>
/// <typeparam name="T">Black box numeric data type.</typeparam>
public interface IBlackBox<T> : IDisposable
    where T : struct
{
    /// <summary>
    /// Gets a vector of input values.
    /// </summary>
    Memory<T> Inputs { get; }

    /// <summary>
    /// Gets a vector of output values.
    /// </summary>
    Memory<T> Outputs { get; }

    /// <summary>
    /// Activate the black box. This causes the black box to accept its inputs and produce output signals
    /// ready for reading from OutputVector.
    /// </summary>
    void Activate();

    /// <summary>
    /// Reset any internal state.
    /// </summary>
    void Reset();
}
