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
            var newJobsXml = new XDocument(new XElement("Jobs"));
            newJobsXml.Save(jobsFilePath);
        }

        var categoriesFilePath = Path.Combine(_xmlFilesDirectory, "Categories.xml");
        if (!File.Exists(categoriesFilePath))
        {
            var newCategoriesXml = new XDocument(
                new XElement("Categories",
                    new XElement("Category", new XAttribute("id", 1), "Work"),
                    new XElement("Category", new XAttribute("id", 2), "Personal"),
                    new XElement("Category", new XAttribute("id", 3), "Home")
                )
            );

            newCategoriesXml.Save(categoriesFilePath);
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


    public void SaveJobs(XDocument document)
    {
        var jobsFilePath = Path.Combine(_xmlFilesDirectory, "Jobs.xml");
        document.Save(jobsFilePath);
    }
}