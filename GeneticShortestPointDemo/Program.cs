// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;

Console.WriteLine("Hello, World!");

// Define the problem data
List<Point> points = new List<Point>() {
    new Point(0, 0),
    new Point(1, 1),
    new Point(2, 3),
    new Point(4, 2),
    new Point(3, 0)
};
Point startPoint = new Point(0, 0);
int populationSize = 100;
int maxGenerations = 20;
double mutationRate = 0.01;

// Create the initial population
List<List<Point>> population = new List<List<Point>>();
for (int i = 0; i < populationSize; i++)
{
    List<Point> solution = points.OrderBy(x => Guid.NewGuid()).ToList();
    solution.Insert(0, startPoint);
    population.Add(solution);
}
// Run the genetic algorithm for a certain number of generations
for (int generation = 0; generation < maxGenerations; generation++)
{
    // Evaluate the fitness of each solution
    Dictionary<List<Point>, double> fitnessValues = new Dictionary<List<Point>, double>();
    foreach (List<Point> solution in population)
    {
        double fitness = Fitness(solution);
        fitnessValues[solution] = fitness;
    }

    // Sort the population by fitness
    population = population.OrderBy(x => fitnessValues[x]).ToList();

    // Print out some information about the best solution in this generation
    Console.WriteLine($"Generation {generation}: best fitness = {fitnessValues[population[0]]}, best solution = {string.Join(" -> ", population[0].Select(x => x.ToString()))}");

    // Create a new population by breeding the fittest individuals
    List<List<Point>> newPopulation = new List<List<Point>>();
    for (int i = 0; i < populationSize; i++)
    {
        // Select two parents using tournament selection
        List<Point> parent1 = TournamentSelection(population, fitnessValues);
        List<Point> parent2 = TournamentSelection(population, fitnessValues);

        // Create a child solution using crossover
        List<Point> child = Crossover(parent1, parent2);

        // Mutate the child solution
        Mutate(child, mutationRate);

        // Add the child solution to the new population
        newPopulation.Add(child);
    }

    // Replace the old population with the new population
    population = newPopulation;
}

// Print out the best solution
Console.WriteLine($"Best solution: {string.Join(" -> ", population[0].Select(x => x.ToString()))}, fitness = {Fitness(population[0])}");
// Draw the best solution to an image with size with 500x500 pixels
Bitmap image = new Bitmap(500, 500);
using Graphics graphics = Graphics.FromImage(image);
graphics.Clear(Color.White);
// scale the points to fit the image
double minX = points.Min(x => x.X);
double maxX = points.Max(x => x.X);
double minY = points.Min(x => x.Y);
double maxY = points.Max(x => x.Y);
double scaleX = 500 / (maxX - minX);
double scaleY = 500 / (maxY - minY);
// mark number of points
for (int i = 0; i < population[0].Count; i++)
{
    graphics.FillEllipse(Brushes.Black, (float) ((population[0][i].X - minX) * scaleX) - 2,
        (float) ((population[0][i].Y - minY) * scaleY) - 2, 4, 4);
    graphics.DrawString(i.ToString(), new Font("Arial", 8), Brushes.Black, (float)((population[0][i].X - minX) * scaleX) + 2, (float)((population[0][i].Y - minY) * scaleY) + 2);
}
// draw the lines
for (int i = 0; i < population[0].Count - 1; i++)
{
    graphics.DrawLine(Pens.Red, (float) ((population[0][i].X - minX) * scaleX),
        (float) ((population[0][i].Y - minY) * scaleY), (float) ((population[0][i + 1].X - minX) * scaleX),
        (float) ((population[0][i + 1].Y - minY) * scaleY));
}
// extend size of image to 10 pixels
// save the image
image.Save("solution.png");

// open the solution image
static double Fitness(List<Point> solution)
{
    // Calculate the total distance of the solution
    double totalDistance = 0;
    for (int i = 0; i < solution.Count - 1; i++)
    {
        totalDistance += DistanceTo(solution[i],solution[i + 1]);
    }
    return totalDistance;
}

static double DistanceTo( Point point1, Point point2)
{
    // Calculate the distance between two points
    return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
}
// Mutate a solution by swapping two points
static void Mutate(List<Point> solution, double mutationRate)
{
    for (int i = 0; i < solution.Count; i++)
    {
        if (RandomNumberGenerator.GetInt32(0, 100) < mutationRate * 100)
        {
            int j = RandomNumberGenerator.GetInt32(1, solution.Count);
            (solution[i], solution[j]) = (solution[j], solution[i]);
        }
    }
}
// Crossover
static List<Point> Crossover(List<Point> parent1, List<Point> parent2)
{
    // Create a child solution by randomly selecting points from the parents
    List<Point> child = new List<Point>();
    foreach (Point point in parent1)
    {
        if (RandomNumberGenerator.GetInt32(0, 100) < 50)
        {
            child.Add(point);
        }
    }
    foreach (Point point in parent2)
    {
        if (!child.Contains(point))
        {
            child.Add(point);
        }
    }
    return child;
}
// Tournament selection
static List<Point> TournamentSelection(List<List<Point>> population, Dictionary<List<Point>, double> fitnessValues)
{
    // Select a random individual from the population
    List<Point> individual = population[RandomNumberGenerator.GetInt32(0, population.Count)];

    // Select another random individual from the population
    List<Point> otherIndividual = population[RandomNumberGenerator.GetInt32(0, population.Count)];

    // Return the individual with the best fitness
    if (fitnessValues[individual] < fitnessValues[otherIndividual])
    {
        return individual;
    }
    else
    {
        return otherIndividual;
    }
}