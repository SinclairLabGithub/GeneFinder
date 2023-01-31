using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class ProblemSolver
    {
        Problem problem;
        CodingScore score;

        public ProblemSolver(Problem problem)
        {
            this.problem = problem;
        }

        private CodingScore Optimize(bool coding)
        {
            int maxIterations = 0;
            //Swarm swarm1 = new Swarm(1, 1, 1, coding, problem);
            //Swarm swarm2 = new Swarm(56, 100, 5, coding, problem);
            Swarm swarm1 = new Swarm(1, 45, 5, coding, problem);
            Swarm swarm2 = new Swarm(56, 100, 5, coding, problem);
            //Console.WriteLine ("swarm1={0}", swarm1);
            //Console.WriteLine ("swarm2={0}", swarm2);
            Swarm.SwapHalf(swarm1, swarm2);
            for (int i = 0; i < maxIterations; ++i)
            {
                swarm1.Update();
                swarm2.Update();
                //Console.WriteLine("swarm1={0}", swarm1);
                //Console.WriteLine("swarm2={0}", swarm2);
                Swarm.SwapHalf(swarm1, swarm2);
            }
            //return swarm1.BestScore;
            if (swarm1.BestScore.GlobalScore > swarm2.BestScore.GlobalScore)
                return swarm1.BestScore;
            else
                return swarm2.BestScore;
        }

        public CodingScore Solve()
        {
            CodingScore coding = Optimize(true);
            CodingScore nonCoding = Optimize(false);
            score = coding - nonCoding;
            return score;
        }
    }
}
