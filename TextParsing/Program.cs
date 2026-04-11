using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CsvHelper;
using System.Globalization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DataManagerApp
{
    public class Person
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        
        public override string ToString()
        {
            return id + ":  " + firstName + "  " + lastName;
        }
    }

    class Program
    {
        // список хранения
        static List<Person> people = new List<Person>();
        
        // счетчик для новых id
        static int nextId = 1;

        static void Main(string[] args)
        {
            //main menu
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                    Console.WriteLine("=== программа для работы с данными ===");
                Console.WriteLine("1. считать данные из файла");
                Console.WriteLine("2. записать данные в файл");
                Console.WriteLine("3. вывести данные на экран");
                Console.WriteLine("4. сортировка");
                Console.WriteLine("5. поиск по имени или фамилии");
                Console.WriteLine("6. добавить нового человека");
                Console.WriteLine("7. удалить человека по id");
                Console.WriteLine("8. изменить данные человека");
                Console.WriteLine("9. выход");
                Console.Write("выбери пункт: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        readDataFromFile();
                        break;
                    case "2":
                        writeDataToFile();
                        break;
                    case "3":
                        displayData();
                        break;
                    case "4":
                        sortData();
                        break;
                    case "5":
                        searchData();
                        break;
                    case "6":
                        addPerson();
                        break;
                    case "7":
                        deletePerson();
                        break;
                    case "8":
                        editPerson();
                        break;
                    case "9":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("неверный выбор, нажми любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        //чтение из файла 
        
        static void readDataFromFile()
        {
            Console.Write("введи имя файла: ");
            string filename = Console.ReadLine();
            Console.Write("формат (csv/json/xml/yaml): ");
            string format = Console.ReadLine().ToLower();
            
            try
            {
                //проверка на то, существует ли файл или нет
                if (!File.Exists(filename))
                {
                    //не существует - текст. ошибка
                    Console.WriteLine("ошибка: файл не существует");
                    Console.ReadKey();
                    return;
                }
                
                switch (format)
                {
                    case "csv":
                        people = readCsv(filename);
                        break;
                    case "json":
                        people = readJson(filename);
                        break;
                    case "xml":
                        people = readXml(filename);
                        break;
                    case "yaml":
                        people = readYaml(filename);
                        break;
                    default:
                        Console.WriteLine("неизвестный формат");
                        Console.ReadKey();
                        return;
                }
                
                
                
                
                
                if (people.Count > 0)
                {
                    nextId = people.Max(p => p.id) + 1;
                }
                
                Console.WriteLine("загружено " + people.Count + " записей");
            }
            catch (Exception ex)
            {
             
                Console.WriteLine("ошибка при чтении: " + ex.Message);
                File.AppendAllText("error.log", DateTime.Now + ": " + ex.Message + Environment.NewLine);
            }
            
            Console.ReadKey();
        }
        
        // запись
        
        static void writeDataToFile()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("нет данных для сохранения");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи имя файла: ");
            string filename = Console.ReadLine();
            Console.Write("формат (csv/json/xml/yaml): ");
            string format = Console.ReadLine().ToLower();
            
            try
            {
                switch (format)
                {
                    case "csv":
                        writeCsv(filename, people);
                        break;
                    case "json":
                        writeJson(filename, people);
                        break;
                    case "xml":
                        writeXml(filename, people);
                        break;
                    case "yaml":
                        writeYaml(filename, people);
                        break;
                    default:
                        Console.WriteLine("неизвестный формат");
                        Console.ReadKey();
                        return;
                }
                
                Console.WriteLine("данные сохранены");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ошибка при сохранении: " + ex.Message);
                File.AppendAllText("error.log", DateTime.Now + ": " + ex.Message + Environment.NewLine);
            }
            
            Console.ReadKey();
        }
        
        //вывод на экран
        
        static void displayData()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("список пуст");
            }
            else
            {
                Console.WriteLine("список людей:");
                foreach (Person p in people)
                {
                    Console.WriteLine(p);
                }
            }
            Console.ReadKey();
        }
        
        // sort
        
        static void sortData()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("нет данных для сортировки");
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine("сортировать по:");
            Console.WriteLine("1 - id");
            Console.WriteLine("2 - имени");
            Console.WriteLine("3 - фамилии");
            Console.Write("выбери: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    people = people.OrderBy(p => p.id).ToList();
                    Console.WriteLine("отсортировано по id");
                    break;
                case "2":
                    people = people.OrderBy(p => p.firstName).ToList();
                    Console.WriteLine("отсортировано по имени");
                    break;
                case "3":
                    people = people.OrderBy(p => p.lastName).ToList();
                    Console.WriteLine("отсортировано по фамилии");
                    break;
                default:
                    Console.WriteLine("неверный выбор");
                    break;
            }
            
            Console.ReadKey();
        }
        
        //search
        
        static void searchData()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("нет данных для поиска");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи текст для поиска (ищет по имени и фамилии): ");
            string searchText = Console.ReadLine().ToLower();
            
            // ищем людей у которых имя или фамилия содержат искомый текст
            List<Person> results = people.Where(p => 
                p.firstName.ToLower().Contains(searchText) || 
                p.lastName.ToLower().Contains(searchText)
            ).ToList();
            
            if (results.Count == 0)
            {
                Console.WriteLine("ничего не найдено");
            }
            else
            {
                Console.WriteLine("найдено " + results.Count + " записей:");
                foreach (Person p in results)
                {
                    Console.WriteLine(p);
                }
            }
            
            Console.ReadKey();
        }
        
        //добавление
        
        static void addPerson()
        {
            Console.Write("введи имя: ");
            string firstName = Console.ReadLine();
            Console.Write("введи фамилию: ");
            string lastName = Console.ReadLine();
            
            //создание
            Person newPerson = new Person
            {
                id = nextId,
                firstName = firstName,
                lastName = lastName
            };
            
            people.Add(newPerson);
            nextId++;
            
            Console.WriteLine("человек добавлен, id = " + newPerson.id);
            Console.ReadKey();
        }
        
        //delete
        
        static void deletePerson()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("нет данных для удаления");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи id человека для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("неверный id");
                Console.ReadKey();
                return;
            }
            
            
            
            
            
            
            
            
            //поиск человека по id 
            Person personToDelete = people.FirstOrDefault(p => p.id == id);
            
            if (personToDelete == null)
            {
                Console.WriteLine("человек с id " + id + " не найден");
            }
            else
            {
                people.Remove(personToDelete);
                Console.WriteLine("человек удален");
            }
            
            Console.ReadKey();
        }
        
        //изменение данных
        
        static void editPerson()
        {
            if (people.Count == 0)
            {
                Console.WriteLine("нет данных для изменения");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи id человека для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("неверный id");
                Console.ReadKey();
                return;
            }
            
            // ищем человека po id
            Person personToEdit = people.FirstOrDefault(p => p.id == id);
            
            if (personToEdit == null)
            {
                Console.WriteLine("человек с id " + id + " не найден");
            }
            else
            {
                Console.Write("новое имя (старое: " + personToEdit.firstName + "): ");
                string newFirstName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newFirstName))
                {
                    personToEdit.firstName = newFirstName;
                }
                
                Console.Write("новая фамилия (старая: " + personToEdit.lastName + "): ");
                string newLastName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newLastName))
                {
                    personToEdit.lastName = newLastName;
                }
                
                Console.WriteLine("данные изменены");
            }
            
            Console.ReadKey();
        }
        
        //csv read
        
        static List<Person> readCsv(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<Person>().ToList();
            }
        }
        
        //json read
        
        static List<Person> readJson(string filename)
        {
            string json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<List<Person>>(json);
        }
        
        //xml read
        
        static List<Person> readXml(string filename)
        {
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(typeof(List<Person>));
            using (StreamReader reader = new StreamReader(filename))
            {
                return (List<Person>)serializer.Deserialize(reader);
            }
        }
        
        //yaml read
        
        static List<Person> readYaml(string filename)
        {
            string yaml = File.ReadAllText(filename);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<List<Person>>(yaml);
        }
        
        //csv
        
        static void writeCsv(string filename, List<Person> data)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }
        }
        
        //json
        
        static void writeJson(string filename, List<Person> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
        
        //xml
        
        static void writeXml(string filename, List<Person> data)
        {
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(typeof(List<Person>));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, data);
            }
        }
        
        //yaml
        
        static void writeYaml(string filename, List<Person> data)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string yaml = serializer.Serialize(data);
            File.WriteAllText(filename, yaml);
        }
    }
}