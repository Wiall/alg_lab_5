using System;
using System.Linq;

namespace alg_lab_5.Properties
{
    public class BagProblem
    {
        const int P = 500; // Місткість рюкзака
        const int n = 100; // Кількість предметів
        static Random rnd = new Random();
        
        static int[][] GenerateScoutSolutions(int count, int[] weights, int[] values) {
            return Enumerable.Range(0, count)
                .Select(_ => GenerateRandomSolution(weights))
                .ToArray();
        }

        static int[] GenerateRandomSolution(int[] weights) {
            int[] solution = new int[n];
            int totalWeight = 0;

            for (int i = 0; i < n; i++) {
                // Випадкове рішення: чи додаємо предмет до рюкзака
                solution[i] = rnd.Next(0, 2); // 0 або 1

                // Оновлюємо загальну вагу
                totalWeight += solution[i] * weights[i];

                // Якщо вага перевищує місткість, прибираємо предмет
                if (totalWeight > P) {
                    solution[i] = 0;
                    totalWeight -= weights[i];
                }
            }

            return solution;
        }



        static int[] LocalSearch(int[] solution, int[] weights, int[] values) {
            int[] newSolution = (int[])solution.Clone();
            for (int i = 0; i < n; i++) {
                newSolution[i] = Math.Max(0, newSolution[i] + rnd.Next(-1, 2));
                if (CalculateWeight(newSolution, weights) > P) {
                    newSolution[i] = solution[i];
                }
            }
            return newSolution;
        }

        static int CalculateValue(int[] solution, int[] values) {
            return solution.Zip(values, (x, v) => x * v).Sum();
        }

        static int CalculateWeight(int[] solution, int[] weights) {
            return solution.Zip(weights, (x, w) => x * w).Sum();
        }

        static private void PrintSolution(int bestVal, int[] bestSol, int[] w, int[] v)
        
        {
            for (int i = 0; i < w.Length; i++)
            {
                Console.Write($"Object {i}:\tWeight = {w[i]} \t Value = {v[i]} \n");
            }
            
            // Вивід результату
            Console.WriteLine($"Максимальна цiннiсть: {bestVal}");
            Console.WriteLine($"Розподiл предметiв: {string.Join(", ", bestSol)}");
        }

        static public void PerformSolution()
        {
            // Генерація ваг і цінностей
            int[] weights = Enumerable.Range(0, n).Select(_ => rnd.Next(1, 21)).ToArray();
            int[] values = Enumerable.Range(0, n).Select(_ => rnd.Next(2, 31)).ToArray();

            // Параметри бджолиного алгоритму
            int scoutBees = 20;
            int forageBees = 50;
            int sites = 5;
            int maxIterations = 100;

            // Початковий алгоритм
            int[] bestSolution = new int[n];
            int bestValue = 0;

            for (int iter = 0; iter < maxIterations; iter++) {
                // Генерація розвідників
                int[][] scoutSolutions = GenerateScoutSolutions(scoutBees, weights, values);
                int[] fitness = scoutSolutions.Select(sol => CalculateValue(sol, values)).ToArray();

                // Відбір ділянок
                var selectedSites = fitness
                    .Select((f, i) => (Value: f, Index: i))
                    .OrderByDescending(f => f.Value)
                    .Take(sites)
                    .Select(s => scoutSolutions[s.Index])
                    .ToArray();

                // Фуражири покращують ділянки
                foreach (var site in selectedSites) {
                    int[] improvedSolution = LocalSearch(site, weights, values);
                    int improvedValue = CalculateValue(improvedSolution, values);

                    if (improvedValue > bestValue) {
                        bestValue = improvedValue;
                        bestSolution = improvedSolution;
                    }
                }
            }

            PrintSolution(bestValue, bestSolution, weights, values);
        }
    }
}