# Rm old coverage results
Remove-Item .\TestResults\* -Recurse

# Run tests and generate coverage
dotnet test --collect:"XPlat Code Coverage"

# Turn coverage into an html report
reportgenerator -reports:".\TestResults\*\coverage.cobertura.xml" -targetdir:".\TestResults\CoverageReport" -reporttypes:Html

# Get the coverage
$rawCoverage = (Select-String -Path ".\TestResults\CoverageReport\index.html" -Pattern "(<tr><th>(Line|Branch|Method) coverage:<\/th><td>\d{1,3}\.?\d?% \(\d+ of \d+\)<\/td><\/tr>)")
# Lazily reformat by replacing unwanted info with whitespace
$formattedCoverage = $rawCoverage  -replace "(\w:.+index\.html:\d+:)|(<\/?t\w>)", " "
# Print
echo ""
$formattedCoverage
echo ""

# Open the report
echo ""
echo "Open .\TestResults\CoverageReport\index.html to view the full report."
echo ""