var currentTheme = localStorage.getItem("theme");

var supportedThemes = ["default", "dark", "software", "humanistic", "standard", "material" ];

console.log("current theme: " + currentTheme);
if (!currentTheme || !supportedThemes.includes(currentTheme)) {
    console.log("setting current theme to material")
    currentTheme = "material";
    localStorage.setItem("theme", currentTheme);
}

var activeSheet = document.getElementById("active-stylesheet");
var activeSheetBase = document.getElementById("active-stylesheet-base");

activeSheet.setAttribute("href", "_content/Radzen.Blazor/css/" + currentTheme + ".css");
activeSheetBase.setAttribute("href", "_content/Radzen.Blazor/css/" + currentTheme + "-base.css");

window.clipboardCopy = {
    copyText: function(text) {
        navigator.clipboard.writeText(text).then(function () {
            console.log("Copied to clipboard!");
        })
            .catch(function (error) {
                alert(error);
            });
    }
};

async function downloadFileFromStream(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);

    triggerFileDownload(fileName, url);

    URL.revokeObjectURL(url);
}

function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}