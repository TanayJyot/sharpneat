﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.NeuralNets;

// TODO: Review class, as it doesn't appear to be used currently.

/// <summary>
/// Represents network activation schemes.
/// </summary>
public sealed class NetworkActivationScheme
{
    /// <summary>
    /// Private constructor to restrict construction to static factory methods.
    /// </summary>
    private NetworkActivationScheme()
    {
    }

    /// <summary>
    /// Gets a value indicating whether the network is acyclic or not (cyclic).
    /// </summary>
    public bool AcyclicNetwork { get; init; }

    /// <summary>
    /// Gets the number of activation cycles to perform per overall activation of
    /// a cyclic network. Used for cyclic networks only.
    /// </summary>
    public int CyclesPerActivation { get; init; }

        /// <summary>
    /// Create an activation scheme for acyclic networks.
    /// </summary>
    /// <returns>A new instance of <see cref="NetworkActivationScheme"/>.</returns>
    public static NetworkActivationScheme CreateAcyclicScheme()
    {
        NetworkActivationScheme scheme = new()
        {
            AcyclicNetwork = true
        };

        return scheme;
    }

    /// <summary>
    /// Create an activation scheme for cyclic networks.
    /// </summary>
    /// <param name="cyclesPerActivation">The number of activation cycles to perform per overall activation of
    /// the cyclic network.</param>
    /// <returns>A new instance of <see cref="NetworkActivationScheme"/>.</returns>
    public static NetworkActivationScheme CreateCyclicScheme(int cyclesPerActivation)
    {
        NetworkActivationScheme scheme = new()
        {
            AcyclicNetwork = false,
            CyclesPerActivation = cyclesPerActivation
        };

        return scheme;
    }
}
