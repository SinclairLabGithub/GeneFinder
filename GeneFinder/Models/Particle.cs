using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Particle
    {
        double position;
        double velocity;
        CodingScore score;
        double bestFitness;
        double bestPosition;
        bool coding;
        Problem problem;
        double[][][] substitution;

        public double Position
        {
            get
            {
                return this.position;
            }
        }

        public double Fitness
        {
            get
            {
                return this.score.GlobalScore;
            }
            set
            {
                score.GlobalScore = value;
            }
        }

        public CodingScore Score
        {
            get
            {
                return this.score;
            }
        }

        public Particle(double position, bool coding, Problem problem)
        {
            this.position = position;
            this.coding = coding;
            this.problem = problem;

            substitution = new double[4][][];
            for (int iSubject = 0; iSubject < 4; ++iSubject)
            {
                substitution[iSubject] = new double[64][];
                for (int i = 0; i < 64; ++i)
                {
                    substitution[iSubject][i] = new double[64];
                }
            }
            velocity = 0;
            bestPosition = position;
            score = new CodingScore(problem.Length);
            CalculateFitness();
            bestFitness = this.Fitness;
        }

        private void UpdateVelocity(double bestSwarmPosition)
        {
            velocity += Parameters.getLocalFactor() * (bestPosition - position);
            velocity += Parameters.getGlobalFactor() * (bestSwarmPosition - position);
        }

        private void UpdatePosition()
        {
            while (position + velocity < 0)
                velocity /= 2;
            position += velocity;
        }

        private void UpdateBestPosition()
        {
            if (this.Fitness > bestFitness)
            {
                bestFitness = this.Fitness;
                bestPosition = position;
            }
        }

        private void CalculateFitness()
        {
            CodonRateMatrices.CalculateSubstitutionMatrices(coding, position, substitution);
            double likelihood = 0;
            for (int iCodon = 0; iCodon < problem.Length; ++iCodon)
            {
                score[iCodon] = 10*Math.Log10(codonLikelihood(iCodon));
                if (score[iCodon] != CodingScore.invalidScore)
                    likelihood += score[iCodon];
            }
            this.Fitness = likelihood;
        }

        private double codonLikelihood(int iCodon)
        {
            int h, c, r;
            h = problem.Human[iCodon].Number;
            c = problem.Chimp[iCodon].Number;
            r = problem.Rhesus[iCodon].Number;
            if ((h == -1) || (c == -1) || (r == -1))
                return CodingScore.invalidScore;

            double[] distribution = CodonRateMatrices.GetDistribution(coding);
            double[][] substitutionU = substitution[(int)Problem.Subject.U];
            double[][] substitutionH = substitution[(int)Problem.Subject.H];
            double[][] substitutionC = substitution[(int)Problem.Subject.C];
            double[][] substitutionR = substitution[(int)Problem.Subject.R];

            double likelihood = 0;
            for (int v = 0; v < 64; ++v)
            {
                double marginalLikelihood = 0;
                for (int u = 0; u < 64; ++u)
                    marginalLikelihood += substitutionU[v][u] * substitutionC[u][c] * substitutionH[u][h];
                likelihood += distribution[v] * substitutionR[v][r] * marginalLikelihood;
            }
            return likelihood;
        }

        public void Update(double bestSwarmPosition)
        {
            UpdateVelocity(bestSwarmPosition);
            UpdatePosition();
            CalculateFitness();
            UpdateBestPosition();
        }
    }
}
