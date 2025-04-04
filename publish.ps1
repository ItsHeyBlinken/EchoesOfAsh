# Publish script for GitHub Pages

# Set the repository name - change this to match your GitHub repository name
$repoName = "EchoesOfAsh"

# Clean and publish the Blazor app
dotnet publish EchoesOfAshWeb -c Release -o publish

# Create the .nojekyll file (if it doesn't exist)
New-Item -Path "publish/wwwroot/.nojekyll" -ItemType File -Force

# Update the base href in index.html for GitHub Pages
$indexPath = "publish/wwwroot/index.html"
$indexContent = Get-Content -Path $indexPath -Raw
$indexContent = $indexContent -replace '<base href="/" />', "<base href=`"/$repoName/`" />"
Set-Content -Path $indexPath -Value $indexContent

Write-Host "Blazor app published successfully to the 'publish' folder."
Write-Host "Copy the contents of the 'publish/wwwroot' folder to your GitHub Pages repository."
