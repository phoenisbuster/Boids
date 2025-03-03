//usage:
function readTextFile(file, callback) {
  var rawFile = new XMLHttpRequest();
  rawFile.overrideMimeType("application/json");
  rawFile.open("GET", file, true);
  rawFile.onreadystatechange = function () {
    if (rawFile.readyState === 4 && rawFile.status == "200") {
      callback(rawFile.responseText);
    }
  };
  rawFile.send(null);
}

function saveParameterToLS(name, value) {
  if (value) {
    window.localStorage.setItem(name, value);
  }
}

readTextFile("package.json", function (text) {
  var data = JSON.parse(text);
  saveParameterToLS("package", text);
});

