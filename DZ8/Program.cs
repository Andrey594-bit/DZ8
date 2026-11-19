using System;
using System.Collections.Generic;
using TaskManager;

namespace TumakovLab
{
    class Program
    {
        static void Main()
        {
            List<Song> songs = new List<Song>();

            for (int i = 0; i < 4; i++)
            {
                Console.Write("Название: ");
                string name = Console.ReadLine();

                Console.Write("Автор: ");
                string author = Console.ReadLine();

                Song prev = songs.Count > 0 ? songs[songs.Count - 1] : null;
                songs.Add(new Song(name, author, prev));
            }

            foreach (Song s in songs)
                s.Print();

            Console.WriteLine(songs[0].Equals(songs[1]) ? "совпадают" : "не совпадают");

            // Проект
            List<Person> team = new List<Person>();
            for (int i = 1; i <= 10; i++)
                team.Add(new Person("Сотрудник" + i));

            Person customer = new Person("Заказчик");
            Person teamLead = team[0];

            Project project = new Project(
                "Разработка Task Manager",
                DateTime.Today,
                DateTime.Today.AddMonths(2),
                customer,
                teamLead);

            Console.WriteLine(project);
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                Person assignee = team[i];
                TaskItem task = new TaskItem(
                    "Задача №" + (i + 1),
                    DateTime.Today.AddDays(5 + i),
                    teamLead);
                task.Assignee = assignee;
                project.AddTask(task);
               
            }

            TaskItem extra = new TaskItem("Лишняя задача", DateTime.Today.AddDays(3), teamLead);
            extra.Assignee = team[1];
            project.AddTask(extra);
            

            Console.WriteLine("Задачи до старта проекта:");
            for (int i = 0; i < project.Tasks.Count; i++)
                Console.WriteLine("  " + project.Tasks[i]);
            Console.WriteLine();

            try
            {
                TaskItem bad = new TaskItem("Задача после старта", DateTime.Today, teamLead);
                bad.Assignee = team[2];
                project.Start();
                project.AddTask(bad);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            Console.WriteLine();

            Console.WriteLine("Проект переведён в статус: " + project.Status);
            Console.WriteLine();

            try
            {
                Workflow.Reject(extra);
                Console.WriteLine("Задача \"" + extra.Description + "\" отклонена (исполнитель: —)");
                project.RemoveTask(extra);
                Console.WriteLine("Задача \"" + extra.Description + "\" удалена тимлидом");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            Console.WriteLine();

            for (int i = 0; i < project.Tasks.Count; i++)
            {
                TaskItem task = project.Tasks[i];

                if (i == 0)
                {
                    Workflow.Delegate(task, team[5]);
                    Console.WriteLine("Задача \"" + task.Description + "\" делегирована на " + task.Assignee);
                }
                else if (i == 1)
                {
                    string nameBefore = task.Assignee == null ? "—" : task.Assignee.Name;
                    Workflow.Reject(task);
                    Console.WriteLine("Задача \"" + task.Description + "\" отклонена (исполнитель: " + nameBefore + ")");
                    Workflow.Delegate(task, team[6]);
                    Console.WriteLine("  → переназначена на " + task.Assignee);
                }

                Workflow.TakeToWork(task);

                Report report = new Report("Выполнено исполнителем " + task.Assignee, task.Assignee);
                Workflow.SubmitReport(task, report);

                if (i == 2)
                {
                    try
                    {
                        Workflow.ApproveReport(task, team[3]);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("Ошибка: " + ex.Message);
                    }

                    Workflow.ReturnReport(task, task.Initiator);
                    Console.WriteLine("Отчёт по задаче \"" + task.Description + "\" возвращён на доработку");

                    Report fixedReport = new Report("Исправлено исполнителем " + task.Assignee, task.Assignee);
                    Workflow.SubmitReport(task, fixedReport);
                }

                Workflow.ApproveReport(task, task.Initiator);
            }

            Console.WriteLine();
            Console.WriteLine("Итог по задачам:");
            for (int i = 0; i < project.Tasks.Count; i++)
            {
                Console.WriteLine("  " + project.Tasks[i]);
                if (project.Tasks[i].Report != null)
                    Console.WriteLine("     " + project.Tasks[i].Report);
            }

            Console.WriteLine();
            Console.WriteLine("Задачи каждого члена команды:");
            for (int i = 0; i < team.Count; i++)
            {
                Person p = team[i];
                string list = "";
                for (int j = 0; j < p.Tasks.Count; j++)
                {
                    if (j > 0) list += ", ";
                    list += p.Tasks[j].Description;
                }
                if (list == "") list = "—";
                Console.WriteLine("  " + p + ": " + list);
            }

            bool allHaveTasks = true;
            for (int i = 0; i < team.Count; i++)
            {
                if (team[i].Tasks.Count == 0)
                {
                    allHaveTasks = false;
                    Console.WriteLine("Ошибка: у " + team[i] + " нет ни одной задачи.");
                }
            }
            if (allHaveTasks)
                Console.WriteLine("Проверка пройдена: у каждого члена команды есть минимум одна задача.");

            project.TryClose();
            Console.WriteLine();
            Console.WriteLine("Статус проекта после выполнения всех задач: " + project.Status);
        }
    
    }
}