using AventStack.ExtentReports;
using TechTalk.SpecFlow;

namespace NASA.Automation.Tests.Hooks
{
    [Binding]
    public sealed class ExtentReportHooks
    {
        private static AventStack.ExtentReports.ExtentReports _extent;
        private static ExtentTest _feature;
        private static ExtentTest _scenario;
        private readonly ScenarioContext _context;

        public ExtentReportHooks(ScenarioContext context)
        {
            _context = context;
        }

        [BeforeTestRun]
        public static void InitializeReport()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var reportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtentReports");
            Directory.CreateDirectory(reportDir);

            var reportPath = Path.Combine(reportDir, $"NASA_Automation_Report_{timestamp}.html");

            var htmlReporter = new AventStack.ExtentReports.Reporter.ExtentV3HtmlReporter(reportPath);
            htmlReporter.Config.DocumentTitle = "NASA Automation Test Report";
            htmlReporter.Config.ReportName = "NASA API + UI Validation";
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

            _extent = new AventStack.ExtentReports.ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("Environment", "QA");
            _extent.AddSystemInfo("Application", "NASA Public APIs");
            _extent.AddSystemInfo("Tester", "Yinka Merit");

            Console.WriteLine($"Extent report initialized at: {reportPath}");
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            _feature = _extent.CreateTest(featureContext.FeatureInfo.Title)
                .AssignCategory("Feature")
                .AssignAuthor("NASA Automation Suite");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var scenarioTitle = _context.ScenarioInfo.Title;
            if (_context.ScenarioInfo.Arguments != null && _context.ScenarioInfo.Arguments.Count > 0)
            {
                var argsList = new List<string>();

                foreach (System.Collections.DictionaryEntry arg in _context.ScenarioInfo.Arguments)
                {
                    argsList.Add($"{arg.Key}: {arg.Value}");
                }

                scenarioTitle += $" [{string.Join(", ", argsList)}]";
            }

            _scenario = _feature.CreateNode(scenarioTitle)
                                .AssignCategory(string.Join(", ", _context.ScenarioInfo.Tags));

            _scenario.Log(Status.Info, $"Starting Scenario: {scenarioTitle}");
        }

        [AfterStep]
        public void InsertReportingSteps()
        {
            var step = _context.StepContext.StepInfo;
            var stepText = $"{step.StepDefinitionType} {step.Text}";

            var status = _context.TestError == null ? Status.Pass : Status.Fail;

            if (status == Status.Pass)
            {
                _scenario.Log(status, $"{stepText}");
            }
            else
            {
                _scenario.Log(status, $"{stepText} — Error: {_context.TestError.Message}");

                if (_context.TestError.StackTrace != null)
                {
                    _scenario.Log(Status.Error, $"StackTrace:\n{_context.TestError.StackTrace}");
                }
            }
        }

        [AfterTestRun]
        public static void TearDownReport()
        {
            _extent.Flush();
            Console.WriteLine("Extent report generated successfully.");
        }
    }
}
