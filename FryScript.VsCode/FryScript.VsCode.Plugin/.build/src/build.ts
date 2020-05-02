import { spawn } from 'child_process';
import { platform } from 'process';

let runtime = undefined;

switch (platform) {
    case 'win32':
        runtime = 'win10-x64';
        break;
    case 'linux':
        runtime = 'linux-x64';
        break;
}

let process = spawn('dotnet', ['publish', '../FryScript.VsCode.LanguageServer', '-r', runtime, '-f', 'netcoreapp3.1', '-c', 'Debug']);

process.stdout.on('data', data => {
    console.log(`${data}`);
});

process.stderr.on('data', data => {
    console.log(`${data}`);
});
// exec('"dotnet publish ../LanguageServer -r win10-x64 -f netcoreapp2.2 -r Release"', (err, stdout, stderr) => {
//     console.log(stdout);
//     console.log(stderr);
// });
