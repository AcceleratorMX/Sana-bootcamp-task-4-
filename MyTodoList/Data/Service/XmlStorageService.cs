using System.Xml.Linq;

namespace MyTodoList.Data.Service;

public class XmlStorageService
{
    private readonly string _xmlFilesDirectory;

    public XmlStorageService(string xmlFilesDirectory)
    {
        _xmlFilesDirectory = xmlFilesDirectory;
        EnsureFilesExist();
    }

    private void EnsureFilesExist()
    {
        var jobsFilePath = Path.Combine(_xmlFilesDirectory, "Jobs.xml");
        if (!File.Exists(jobsFilePath))
        {
            var defaultJobsXml = new XDocument(new XElement("Jobs"));
            defaultJobsXml.Save(jobsFilePath);
        }

        var categoriesFilePath = Path.Combine(_xmlFilesDirectory, "Categories.xml");
        if (!File.Exists(categoriesFilePath))
        {
            var defaultCategoriesXml = new XDocument(
                new XElement("Categories",
                    new XElement("Category", new XAttribute("id", 1), "Дім"),
                    new XElement("Category", new XAttribute("id", 2), "Робота"),
                    new XElement("Category", new XAttribute("id", 3), "Навчання")
                )
            );

            defaultCategoriesXml.Save(categoriesFilePath);
        }
    }

    public XDocument LoadJobs()
    {
        var filePath = Path.Combine(_xmlFilesDirectory, "Jobs.xml");
        return XDocument.Load(filePath);
    }

    public XDocument LoadCategories()
    {
        var filePath = Path.Combine(_xmlFilesDirectory, "Categories.xml");
        return XDocument.Load(filePath);
    }


    public void SaveJobs(XDocument? document)
    {
        var filePath = Path.Combine(_xmlFilesDirectory, "Jobs.xml");
        document?.Save(filePath);
    }
}