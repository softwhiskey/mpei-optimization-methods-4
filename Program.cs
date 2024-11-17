using System;
using System.Linq;
using Microsoft.SolverFoundation.Services;

namespace ConsoleApp2
{
    class Program
    {
        public static void Main(string[] args)
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            // * Объявляем переменные решения с ограничениями неотрицательности
            Decision x1 = new Decision(domain: Domain.RealNonnegative, name: "x1");
            Decision x2 = new Decision(domain: Domain.RealNonnegative, name: "x2");
            Decision x3 = new Decision(domain: Domain.RealNonnegative, name: "x3");
            Decision x4 = new Decision(domain: Domain.RealNonnegative, name: "x4");
            Decision x5 = new Decision(domain: Domain.RealNonnegative, name: "x5");

            // * Добавляем переменные в модель
            model.AddDecisions(x1, x2, x3, x4, x5);

            // * Определяем целевую функцию (максимизация)
            model.AddGoal(name: "Цель", direction: GoalKind.Maximize, goal: -45 * x1 + 65 * x2 + 2 * x4 - 3 * x5);

            // * Добавляем ограничения
            model.AddConstraint(name: "c1", constraint: 15 * x1 + 18 * x2 + 34 * x4 - 22 * x5 == 56);
            model.AddConstraint(name: "c2", constraint: 2 * x1 + 7 * x3 - 4 * x4 + 3 * x5 >= 91);
            model.AddConstraint(name: "c3", constraint: 0.2 * x1 + 0.8 * x2 + 1.5 * x3 + 0.9 * x4 + 4 * x5 <= 26);
            model.AddConstraint(name: "c4", constraint: 1.8 * x1 - 42 * x2 + 6.4 * x3 + 3 * x5 == 15);

            Solution solution = context.Solve();

            // * Проверяем, найдено ли оптимальное решение
            if (solution.Quality == SolverQuality.Optimal)
            {
                Console.WriteLine("Решение найдено:");
                Console.WriteLine($"x1 = {x1.GetDouble()}");
                Console.WriteLine($"x2 = {x2.GetDouble()}");
                Console.WriteLine($"x3 = {x3.GetDouble()}");
                Console.WriteLine($"x4 = {x4.GetDouble()}");
                Console.WriteLine($"x5 = {x5.GetDouble()}");
                Console.WriteLine($"L(X) = {model.Goals.FirstOrDefault().ToDouble()}");
            }
            else
            {
                Console.WriteLine("Решений нету");
            }

            Console.ReadLine();
        }
    }
}