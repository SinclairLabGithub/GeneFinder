using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Swarm
    {
        Particle[] particles;
        double bestPosition;
        CodingScore bestScore;

        private double BestFitness
        {
            get
            {
                return this.bestScore.GlobalScore;
            }
        }

        public CodingScore BestScore
        {
            get
            {
                return this.bestScore;
            }
        }

        public override string ToString()
        {
            return string.Format("[Swarm: bestPosition={0}, bestScore={1}]\n{2} {3}", bestPosition, bestScore.GlobalScore, bestScore[0], bestScore[bestScore.Length - 1]);
        }

        public Swarm(double start, double end, int numParticles, bool coding, Problem problem)
        {
            double step = (end - start) / (numParticles - 1);
            double position = start;

            particles = new Particle[numParticles];
            for (int i = 0; i < numParticles - 1; ++i)
            {
                particles[i] = new Particle(position, coding, problem);
                position += step;
            }
            particles[numParticles - 1] = new Particle(end, coding, problem);

            bestPosition = particles[0].Position;
            bestScore = particles[0].Score;
            for (int i = 0; i < numParticles; ++i)
            {
                if (particles[i].Fitness > this.bestScore.GlobalScore)
                {
                    bestScore = (CodingScore)particles[i].Score.Copy();
                    bestPosition = particles[i].Position;
                }
            }
        }

        public void Update()
        {
            foreach (Particle particle in particles)
            {
                particle.Update(bestPosition);
            }
            foreach (Particle particle in particles)
            {
                if (particle.Fitness > this.BestFitness)
                {
                    bestScore = (CodingScore)particle.Score.Copy();
                    bestPosition = particle.Position;
                }
            }
        }

        static public void SwapHalf(Swarm left, Swarm right)
        {
            if (left.particles.Length != right.particles.Length)
                return;
            Particle temp;
            for (int i = 0; i < left.particles.Length; i += 2)
            {
                temp = left.particles[i];
                left.particles[i] = right.particles[i];
                right.particles[i] = temp;
            }
        }
    }
}
