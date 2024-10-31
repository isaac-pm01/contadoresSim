using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreadedCounters
{
    class Program
    {
        static Dictionary<int, Thread> activeCounters = new Dictionary<int, Thread>();
        static Dictionary<int, bool> isCounterRunning = new Dictionary<int, bool>();
        static Dictionary<int, int> counterValues = new Dictionary<int, int>();

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        StartNewCounter();
                        break;
                    case "2":
                        StopCounter();
                        break;
                    case "3":
                        ShowCountersStatus();
                        break;
                    case "4":
                        exit = true;
                        StopAllCounters();
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Menú de Contadores ===");
            Console.WriteLine("1. Iniciar un nuevo contador");
            Console.WriteLine("2. Detener un contador específico");
            Console.WriteLine("3. Ver estado de los contadores");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");
        }

        static void StartNewCounter()
        {
            Console.Write("Ingrese el número de identificación del contador: ");
            if (int.TryParse(Console.ReadLine(), out int counterId))
            {
                if (activeCounters.ContainsKey(counterId))
                {
                    Console.WriteLine("El contador ya está en marcha. Presione cualquier tecla para continuar.");
                    Console.ReadKey();
                }
                else
                {
                    isCounterRunning[counterId] = true;
                    counterValues[counterId] = 0;

                    Thread newThread = new Thread(() => RunCounter(counterId));
                    activeCounters[counterId] = newThread;
                    newThread.Start();

                    Console.WriteLine("Contador iniciado. Presione cualquier tecla para continuar.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Identificación no válida. Presione cualquier tecla para continuar.");
                Console.ReadKey();
            }
        }

        static void RunCounter(int id)
        {
            while (isCounterRunning[id])
            {
                counterValues[id]++;
                Console.WriteLine($"Contador {id} en progreso: {counterValues[id]}");
                Thread.Sleep(1000); // Incremento cada segundo
            }
        }

        static void StopCounter()
        {
            Console.Write("Ingrese el ID del contador que desea detener: ");
            if (int.TryParse(Console.ReadLine(), out int id) && activeCounters.ContainsKey(id))
            {
                isCounterRunning[id] = false;
                activeCounters[id].Join();
                activeCounters.Remove(id);
                counterValues.Remove(id);

                Console.WriteLine("Contador detenido. Presione cualquier tecla para continuar.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("ID no encontrado o contador no activo. Presione cualquier tecla para continuar.");
                Console.ReadKey();
            }
        }

        static void ShowCountersStatus()
        {
            Console.WriteLine("Estado de todos los contadores en ejecución:");
            foreach (var entry in counterValues)
            {
                Console.WriteLine($"Contador {entry.Key}: {entry.Value}");
            }
            Console.WriteLine("Presione cualquier tecla para regresar al menú.");
            Console.ReadKey();
        }

        static void StopAllCounters()
        {
            foreach (var id in activeCounters.Keys)
            {
                isCounterRunning[id] = false;
            }
            foreach (var thread in activeCounters.Values)
            {
                thread.Join();
            }
            activeCounters.Clear();
            counterValues.Clear();
        }
    }
}
