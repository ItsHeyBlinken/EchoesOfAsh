# Test publish script for GitHub Pages

# Run the publish script
.\publish.ps1

# Check if the publish directory exists
if (Test-Path -Path "publish/wwwroot") {
    Write-Host "✅ Publish directory created successfully."
} else {
    Write-Host "❌ Failed to create publish directory."
    exit 1
}

# Check if the .nojekyll file exists
if (Test-Path -Path "publish/wwwroot/.nojekyll") {
    Write-Host "✅ .nojekyll file created successfully."
} else {
    Write-Host "❌ Failed to create .nojekyll file."
    exit 1
}

# Check if the base href was updated correctly
$indexContent = Get-Content -Path "publish/wwwroot/index.html" -Raw
if ($indexContent -match '<base href="/EchoesOfAsh/" />') {
    Write-Host "✅ Base href updated correctly."
} else {
    Write-Host "❌ Failed to update base href."
    exit 1
}

# Check if essential files exist
$essentialFiles = @(
    "publish/wwwroot/index.html",
    "publish/wwwroot/404.html",
    "publish/wwwroot/_framework/blazor.webassembly.js"
)

foreach ($file in $essentialFiles) {
    if (Test-Path -Path $file) {
        Write-Host "✅ Essential file exists: $file"
    } else {
        Write-Host "❌ Missing essential file: $file"
        exit 1
    }
}

Write-Host ""
Write-Host "✅ All checks passed! Your project is ready for GitHub Pages."
Write-Host "Next steps:"
Write-Host "1. Commit and push these changes to your GitHub repository."
Write-Host "2. Go to your GitHub repository settings and enable GitHub Pages."
Write-Host "3. Set the source to the 'gh-pages' branch."
Write-Host "4. Your site will be available at: https://itsheyblinkn.github.io/EchoesOfAsh/"
