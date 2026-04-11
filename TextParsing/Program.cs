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
    //модель данных
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string registrationDate { get; set; }
        public bool isActive { get; set; }
        
        public override string ToString()
        {
            return id + ": " + name + " | роль: " + role + " | дата регистрации: " + registrationDate + " | активен: " + (isActive ? "да" : "нет");
        }
    }

    class Program
    {
        // список хранения пользователей
        static List<User> users = new List<User>();
        
        // счетчик для новых id
        static int nextId = 1;

        static void Main(string[] args)
        {
            //main menu
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("работа с ланными");
                Console.WriteLine("1. считать данные из файла");
                Console.WriteLine("2. записать данные в файл");
                Console.WriteLine("3. вывести данные на экран");
                Console.WriteLine("4. сортировка");
                Console.WriteLine("5. поиск по имени");
                Console.WriteLine("6. добавить нового пользователя");
                Console.WriteLine("7. удалить пользователя по id");
                Console.WriteLine("8. изменить данные пользователя");
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
                        addUser();
                        break;
                    case "7":
                        deleteUser();
                        break;
                    case "8":
                        editUser();
                        break;
                    case "9":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("неверный выбор");
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
                        users = readCsv(filename);
                        break;
                    case "json":
                        users = readJson(filename);
                        break;
                    case "xml":
                        users = readXml(filename);
                        break;
                    case "yaml":
                        users = readYaml(filename);
                        break;
                    default:
                        Console.WriteLine("неизвестный формат");
                        Console.ReadKey();
                        return;
                }
                
                if (users.Count > 0)
                {
                    nextId = users.Max(u => u.id) + 1;
                }
                
                Console.WriteLine("загружено " + users.Count + " записей");
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
            if (users.Count == 0)
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
                        writeCsv(filename, users);
                        break;
                    case "json":
                        writeJson(filename, users);
                        break;
                    case "xml":
                        writeXml(filename, users);
                        break;
                    case "yaml":
                        writeYaml(filename, users);
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
            if (users.Count == 0)
            {
                Console.WriteLine("список пуст");
            }
            else
            {
                Console.WriteLine("список пользователей:");
                foreach (User u in users)
                {
                    Console.WriteLine(u);
                }
            }
            Console.ReadKey();
        }
        
        // sort
        
        static void sortData()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("нет данных для сортировки");
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine("сортировать по:");
            Console.WriteLine("1 - id");
            Console.WriteLine("2 - имени");
            Console.WriteLine("3 - роли");
            Console.WriteLine("4 - дате регистрации");
            Console.WriteLine("5 - активности");
            Console.Write("выбери: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    users = users.OrderBy(u => u.id).ToList();
                    Console.WriteLine("отсортировано по id");
                    break;
                case "2":
                    users = users.OrderBy(u => u.name).ToList();
                    Console.WriteLine("отсортировано по имени");
                    break;
                case "3":
                    users = users.OrderBy(u => u.role).ToList();
                    Console.WriteLine("отсортировано по роли");
                    break;
                case "4":
                    users = users.OrderBy(u => u.registrationDate).ToList();
                    Console.WriteLine("отсортировано по дате регистрации");
                    break;
                case "5":
                    users = users.OrderBy(u => u.isActive).ToList();
                    Console.WriteLine("отсортировано по активности");
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
            if (users.Count == 0)
            {
                Console.WriteLine("нет данных для поиска");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи текст для поиска (ищет по имени): ");
            string searchText = Console.ReadLine().ToLower();
            
            // ищем пользователей у которых имя содержит искомый текст
            List<User> results = users.Where(u => 
                u.name.ToLower().Contains(searchText)
            ).ToList();
            
            if (results.Count == 0)
            {
                Console.WriteLine("ничего не найдено");
            }
            else
            {
                Console.WriteLine("найдено " + results.Count + " записей:");
                foreach (User u in results)
                {
                    Console.WriteLine(u);
                }
            }
            
            Console.ReadKey();
        }
        
        //добавление пользователя
        
        static void addUser()
        {
            Console.Write("введи имя: ");
            string name = Console.ReadLine();
            Console.Write("введи роль (admin/user/manager): ");
            string role = Console.ReadLine();
            Console.Write("введи дату регистрации (гггг-мм-дд): ");
            string registrationDate = Console.ReadLine();
            Console.Write("пользователь активен? (да/нет): ");
            string activeInput = Console.ReadLine();
            bool isActive = activeInput.ToLower() == "да" || activeInput.ToLower() == "yes" || activeInput.ToLower() == "true";
            
            //создание нового пользователя
            User newUser = new User
            {
                id = nextId,
                name = name,
                role = role,
                registrationDate = registrationDate,
                isActive = isActive
            };
            
            users.Add(newUser);
            nextId++;
            
            Console.WriteLine("пользователь добавлен, id = " + newUser.id);
            Console.ReadKey();
        }
        
        //delete
        
        static void deleteUser()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("нет данных для удаления");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи id пользователя для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("неверный id");
                Console.ReadKey();
                return;
            }
            
            //поиск пользователя по id 
            User userToDelete = users.FirstOrDefault(u => u.id == id);
            
            if (userToDelete == null)
            {
                Console.WriteLine("пользователь с id " + id + " не найден");
            }
            else
            {
                users.Remove(userToDelete);
                Console.WriteLine("пользователь удален");
            }
            
            Console.ReadKey();
        }
        
        //изменение данных пользователя
        
        static void editUser()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("нет данных для изменения");
                Console.ReadKey();
                return;
            }
            
            Console.Write("введи id пользователя для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("неверный id");
                Console.ReadKey();
                return;
            }
            
            // ищем пользователя по id
            User userToEdit = users.FirstOrDefault(u => u.id == id);
            
            if (userToEdit == null)
            {
                Console.WriteLine("пользователь с id " + id + " не найден");
            }
            else
            {
                Console.Write("новое имя (старое: " + userToEdit.name + "): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    userToEdit.name = newName;
                }
                
                Console.Write("новая роль (старая: " + userToEdit.role + "): ");
                string newRole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newRole))
                {
                    userToEdit.role = newRole;
                }
                
                Console.Write("новая дата регистрации (старая: " + userToEdit.registrationDate + "): ");
                string newDate = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDate))
                {
                    userToEdit.registrationDate = newDate;
                }
                
                Console.Write("пользователь активен? (да/нет) (старое: " + (userToEdit.isActive ? "да" : "нет") + "): ");
                string newActive = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newActive))
                {
                    userToEdit.isActive = newActive.ToLower() == "да" || newActive.ToLower() == "yes" || newActive.ToLower() == "true";
                }
                
                Console.WriteLine("данные изменены");
            }
            
            Console.ReadKey();
        }
        
        //csv read
        
        static List<User> readCsv(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<User>().ToList();
            }
        }
        
        //json read
        
        static List<User> readJson(string filename)
        {
            string json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<List<User>>(json);
        }
        
        //xml read
        
        static List<User> readXml(string filename)
        {
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
            using (StreamReader reader = new StreamReader(filename))
            {
                return (List<User>)serializer.Deserialize(reader);
            }
        }
        
        //yaml read
        
        static List<User> readYaml(string filename)
        {
            string yaml = File.ReadAllText(filename);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<List<User>>(yaml);
        }
        
        //csv write
        
        static void writeCsv(string filename, List<User> data)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }
        }
        
        //json write
        
        static void writeJson(string filename, List<User> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
        
        //xml write
        
        static void writeXml(string filename, List<User> data)
        {
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, data);
            }
        }
        
        //yaml write
        
        static void writeYaml(string filename, List<User> data)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string yaml = serializer.Serialize(data);
            File.WriteAllText(filename, yaml);
        }
    }
}