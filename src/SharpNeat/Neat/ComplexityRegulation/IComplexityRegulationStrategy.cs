﻿/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation;

/// <summary>
/// Represents a complexity regulation strategy.
///
/// DetermineMode() is called once per generation. A strategy determines the <see cref="ComplexityRegulationMode"/>
/// that the evolution algorithm search should be in by examining passed in statistics. As such, the simplest valid
/// strategy is to just return <see cref="ComplexityRegulationMode.Complexifying"/>, which results in no complexity
/// regulation taking place.
///
/// Complexity regulation is also known as 'Phased Search'. For more information see:
/// Phased Searching with NEAT: Alternating Between Complexification And Simplification, Colin Green, 2004
/// (http://sharpneat.sourceforge.net/phasedsearch.html).
/// </summary>
public interface IComplexityRegulationStrategy
{
    /// <summary>
    /// Gets the current complexity regulation mode.
    /// </summary>
    ComplexityRegulationMode CurrentMode { get; }

    /// <summary>
    /// Determine the complexity regulation mode that the evolution algorithm search should be in given the
    /// provided evolution algorithm statistics object, and set the current mode to that mode.
    /// </summary>
    /// <param name="eaStats">Evolution algorithm statistics.</param>
    /// <param name="popStats">Population statistics.</param>
    /// <returns>The determined <see cref="ComplexityRegulationMode"/>.</returns>
    ComplexityRegulationMode UpdateMode(
        EvolutionAlgorithmStatistics eaStats,
        PopulationStatistics popStats);
}
