import os
import webbrowser

unit_test_project_name = "SimpleFluentTester.UnitTests"
os.system(f"dotnet test {unit_test_project_name} -c Release /p:CollectCoverage=true /p:CoverletOutput=TestResults/ "
          "/p:CoverletOutputFormat=lcov")

# To be able to use lcov-viewer, you need to have npm installed.
# To install this package, use the following commands:
# npm install -g @lcov-viewer/cli
# yarn global add @lcov-viewer/cli

coverage_file_path = f"./{unit_test_project_name}/TestResults/coverage.info"
report_directory_path = f"./{unit_test_project_name}/TestResults/"
os.system(f"lcov-viewer lcov -o {report_directory_path} {coverage_file_path}")

report_directory_absolute_path = os.path.abspath(report_directory_path)
html_report_file_absolute_path = os.path.join(report_directory_absolute_path, "index.html")
webbrowser.open('file://' + os.path.realpath(html_report_file_absolute_path))
