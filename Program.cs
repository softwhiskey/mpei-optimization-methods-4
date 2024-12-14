using System;
using System.Linq;
using Microsoft.SolverFoundation.Services;

namespace TransportationProblem
{
    // Класс для метода северо-западного угла
    public class NorthwestCornerMethod
    {
        public static int[,] Solve(int[] supply, int[] demand)
        {
            int rows = supply.Length;
            int cols = demand.Length;
            int[,] plan = new int[rows, cols];

            int i = 0, j = 0;
            while (i < rows && j < cols)
            {
                int allocation = Math.Min(supply[i], demand[j]);
                plan[i, j] = allocation;
                supply[i] -= allocation;
                demand[j] -= allocation;

                if (supply[i] == 0) i++;
                else if (demand[j] == 0) j++;
            }

            return plan;
        }
    }

    // Класс для метода минимального элемента
    public class MinimumElementMethod
    {
        public static int[,] Solve(int[] supply, int[] demand, int[,] costs)
        {
            int rows = supply.Length;
            int cols = demand.Length;
            int[,] plan = new int[rows, cols];
            bool[,] used = new bool[rows, cols];

            while (true)
            {
                int minCost = int.MaxValue;
                int minRow = -1, minCol = -1;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (!used[i, j] && costs[i, j] < minCost)
                        {
                            minCost = costs[i, j];
                            minRow = i;
                            minCol = j;
                        }
                    }
                }

                if (minRow == -1) break;

                int allocation = Math.Min(supply[minRow], demand[minCol]);
                plan[minRow, minCol] = allocation;
                supply[minRow] -= allocation;
                demand[minCol] -= allocation;

                used[minRow, minCol] = true;

                if (supply[minRow] == 0)
                {
                    for (int j = 0; j < cols; j++) used[minRow, j] = true;
                }
                if (demand[minCol] == 0)
                {
                    for (int i = 0; i < rows; i++) used[i, minCol] = true;
                }
            }

            return plan;
        }
    }

    // Класс для решения первой задачи линейного программирования
    public class LinearProgrammingSolver
    {
        public static void Solve(int[] supply, int[] demand, int[,] costs)
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            int rows = supply.Length;
            int cols = demand.Length;
            Decision[,] x = new Decision[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    x[i, j] = new Decision(Domain.RealNonnegative, $"x{i + 1}{j + 1}");
                    model.AddDecision(x[i, j]);
                }
            }

            for (int i = 0; i < rows; i++)
            {
                Term supplyConstraint = Model.Sum(new Term[]
                {
                    x[i, 0], x[i, 1], x[i, 2], x[i, 3], x[i, 4]
                });
                model.AddConstraint($"SupplyConstraint_{i + 1}", supplyConstraint <= supply[i]);
            }

            for (int j = 0; j < cols; j++)
            {
                Term demandConstraint = Model.Sum(new Term[]
                {
                    x[0, j], x[1, j], x[2, j]
                });
                model.AddConstraint($"DemandConstraint_{j + 1}", demandConstraint == demand[j]);
            }

            Term objective = Model.Sum(new Term[]
            {
                x[0, 0] * costs[0, 0], x[0, 1] * costs[0, 1], x[0, 2] * costs[0, 2], x[0, 3] * costs[0, 3], x[0, 4] * costs[0, 4],
                x[1, 0] * costs[1, 0], x[1, 1] * costs[1, 1], x[1, 2] * costs[1, 2], x[1, 3] * costs[1, 3], x[1, 4] * costs[1, 4],
                x[2, 0] * costs[2, 0], x[2, 1] * costs[2, 1], x[2, 2] * costs[2, 2], x[2, 3] * costs[2, 3], x[2, 4] * costs[2, 4]
            });

            model.AddGoal("TotalCost", GoalKind.Minimize, objective);

            Solution solution = context.Solve();

            Console.WriteLine("Линейное программирование - решение:");
            if (solution.Quality == SolverQuality.Optimal)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Console.WriteLine($"x{i + 1}{j + 1} = {x[i, j].GetDouble()}");
                    }
                }
                Console.WriteLine($"Общая стоимость: {model.Goals.FirstOrDefault().ToDouble()}");
            }
            else
            {
                Console.WriteLine("Оптимальное решение не найдено.");
            }
        }
    }

    // Основная программа
    class Program
    {
        static void Main(string[] args)
        {
            // Данные для первой задачи (линейное программирование)
            int[] supply1 = { 200, 150, 150 };
            int[] demand1 = { 90, 100, 70, 130, 110 };
            int[,] costs1 = {
                { 12, 15, 21, 14, 17 },
                { 14, 8, 15, 11, 21 },
                { 19, 16, 26, 12, 20 }
            };

            Console.WriteLine("Первая задача: Линейное программирование");
            LinearProgrammingSolver.Solve(supply1, demand1, costs1);

            // Данные для второй задачи (опорный план)
            int[] supply2 = { 210, 170, 65 };
            int[] demand2 = { 125, 90, 130, 100 };
            int[,] costs2 = {
                { 5, 8, 1, 2 },
                { 2, 4, 5, 9 },
                { 9, 2, 3, 1 }
            };

            Console.WriteLine("\nВторая задача: Метод северо-западного угла");
            var northwestPlan = NorthwestCornerMethod.Solve((int[])supply2.Clone(), (int[])demand2.Clone());
            PrintPlan(northwestPlan);

            Console.WriteLine("\nВторая задача: Метод минимального элемента");
            var minElementPlan = MinimumElementMethod.Solve((int[])supply2.Clone(), (int[])demand2.Clone(), costs2);
            PrintPlan(minElementPlan);
            Console.ReadLine();
        }

        static void PrintPlan(int[,] plan)
        {
            int rows = plan.GetLength(0);
            int cols = plan.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(plan[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
