import * as childProcess from "child_process";

console.log("Building language server")

var cp = childProcess.exec("./dotnet publish --framework netcoreapp2.2 -r win-x64", (e, stdout, stderr) => {
    console.log(stdout);
});

console.log(cp.stdout.read());
