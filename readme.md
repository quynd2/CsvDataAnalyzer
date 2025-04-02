# CSV Data Viewer

CSV Data Viewer is an ASP.NET Core MVC application for analyzing and displaying data from CSV files. This application provides functionalities such as displaying data in tables, charts, and calculating statistical values like minimum, maximum, average, and the most expensive hour window.

## System Requirements

- .NET 8.0 SDK or later
- Visual Studio 2022 or Visual Studio Code

## Installation

1. Copy the code and extract it to your machine.

2. Open the project in Visual Studio or Visual Studio Code.

3. Restore the NuGet packages:
    ```bash
    dotnet restore
    ```

4. Run the application:
    ```bash
    dotnet run
    ```

5. Open your browser and navigate to `http://localhost:5000` to view the application.

## Usage

### Upload CSV File

1. Navigate to the home page of the application.
2. Click the "Upload CSV" button to upload your CSV file.
3. The data from the CSV file will be displayed in the "Data Table" and "Summary & Chart" tabs.

### Display Data

- **Tab "Summary & Chart"**: Displays statistical values such as minimum, maximum, average, and the most expensive hour window. Also displays a data chart.
- **Tab "Data Table"**: Displays data from the CSV file in a table format.

### "Show All Data" Functionality

- The "Show All Data" checkbox in both tabs allows you to display all data or only a portion of the data.

## Data Format Note

- The "Date" column in the CSV file must be in the format `dd/MM/yyyy HH:mm` or `dd/MM/yyyy`.

## Service Descriptions

### `ICsvDataService`

Defines methods for working with CSV data:
- `GetCachedData()`: Retrieves cached data from the CSV file.

### `CsvDataService`

Implements `ICsvDataService` and provides methods to read and cache data from CSV files:
- `GetCachedData()`: Retrieves cached data from the CSV file.

### `IDataAnalyzer`

Defines methods for analyzing data:
- `GetMinimum(List<DataRecord> data)`: Calculates the minimum value.
- `GetMaximum(List<DataRecord> data)`: Calculates the maximum value.
- `GetAverage(List<DataRecord> data)`: Calculates the average value.
- `GetMostExpensiveHourWindow(List<DataRecord> data)`: Finds the most expensive hour window.

### `DataAnalyzer`

Implements `IDataAnalyzer` and provides methods to calculate statistical values from the data:
- `GetMinimum(List<DataRecord> data)`: Calculates the minimum value.
- `GetMaximum(List<DataRecord> data)`: Calculates the maximum value.
- `GetAverage(List<DataRecord> data)`: Calculates the average value.
- `GetMostExpensiveHourWindow(List<DataRecord> data)`: Finds the most expensive hour window.

## Contribution

If you would like to contribute to the project, please create a pull request or open an issue on GitHub.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.