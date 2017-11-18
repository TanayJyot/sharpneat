﻿using System;
using System.Collections.Generic;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat.Genome
{
    public static class NestGenomeTestUtils
    {
        public static NeatPopulation<double> CreateNeatPopulation()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 1,
                outputNodeCount: 1,
                isAcyclic: false,
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());

            var genome = CreateNeatGenome(metaNeatGenome);
            var genomeList = new List<NeatGenome<double>>() { genome };
            return new NeatPopulation<double>(metaNeatGenome, genomeList);
        }

        public static NeatGenome<double> CreateNeatGenome(MetaNeatGenome<double> metaNeatGenome)
        {
            var connArr = new ConnectionGene<double>[12];
            connArr[0] = new ConnectionGene<double>(3, 0, 2, 1.0);
            connArr[1] = new ConnectionGene<double>(4, 0, 3, 1.0);
            connArr[2] = new ConnectionGene<double>(5, 0, 4, 1.0);

            connArr[3] = new ConnectionGene<double>(6, 2, 5, 1.0);
            connArr[4] = new ConnectionGene<double>(7, 3, 6, 1.0);
            connArr[5] = new ConnectionGene<double>(8, 4, 7, 1.0);
            
            connArr[6] = new ConnectionGene<double>(9, 5, 8, 1.0);
            connArr[7] = new ConnectionGene<double>(10, 6, 9, 1.0);
            connArr[8] = new ConnectionGene<double>(9, 7, 10, 1.0);

            connArr[9] = new ConnectionGene<double>(11, 8, 1, 1.0);
            connArr[10] = new ConnectionGene<double>(12, 9, 1, 1.0);
            connArr[11] = new ConnectionGene<double>(13, 10, 1, 1.0);

            ConnectionGeneUtils.Sort(connArr);

            var genome = new NeatGenome<double>(metaNeatGenome, 0, 0, connArr);
            return genome;
        }

        public static HashSet<int> GetNodeIdSet(NeatGenome<double> genome)
        {
            var idSet = new HashSet<int>();
            foreach(var connGene in genome.ConnectionGeneArray)
            {
                idSet.Add(connGene.SourceId);
                idSet.Add(connGene.TargetId);
            }
            return idSet;
        }

        public static HashSet<DirectedConnection> GetDirectedConnectionSet(NeatGenome<double> genome)
        {
            var idSet = new HashSet<DirectedConnection>();
            foreach(var connGene in genome.ConnectionGeneArray) {
                idSet.Add(new DirectedConnection(connGene.SourceId, connGene.TargetId));
            }
            return idSet;
        }

    }
}
