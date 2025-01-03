﻿using BenchmarkDotNet.Attributes;
using SharpNeat.Tasks.BinaryTwentyMultiplexer;

#pragma warning disable CA1822 // Mark members as static

namespace SharpNeat.Tasks.BinaryMultiplexer;

public class BinaryTwentyMultiplexerEvaluatorBenchmarks
{
    static readonly BinaryTwentyMultiplexerEvaluator __evaluator = new();
    static readonly NullBlackBox __blackBox = new();

    [Benchmark]
    public void Evaluate()
    {
        __evaluator.Evaluate(__blackBox);
    }

    private sealed class NullBlackBox : IBlackBox<double>
    {
        readonly double[] _inputAndOutputs = new double[22];

        public NullBlackBox()
        {
            Inputs = _inputAndOutputs.AsMemory(0, 21);
            Outputs = _inputAndOutputs.AsMemory(21, 1);
        }

        public Memory<double> Inputs { get; }
        public Memory<double> Outputs { get; }
        public void Activate() {}
        public void Dispose() {}
        public void Reset() {}
    }
}
