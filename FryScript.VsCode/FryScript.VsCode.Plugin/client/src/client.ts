/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
'use strict';

import * as path from 'path';

import { workspace, ExtensionContext } from 'vscode';
import { LanguageClient, LanguageClientOptions, ServerOptions, TransportKind } from 'vscode-languageclient';
import { platform } from 'process';

export function activate(context: ExtensionContext) {

	// The server is implemented in node as a thin wrapper to a dotnet core application
	let debugOptions = { execArgv: ["--nolazy", "--inspect=6009"] };

	let binPath = undefined;

	switch (platform) {
		case "win32":
			binPath = 'bin/netcoreapp5.0/win10-x64/FryScript.VsCode.LanguageServer.exe';
			break;
		case "linux":
			binPath = 'bin/netcoreapp5.0/linux-x64/FryScript.VsCode.LanguageServer';
			break;
		default:
			throw new Error(`Failed to launch language server. Unsupported platform "${process.platform}"`);
	}

	let runCommand = context.asAbsolutePath(binPath);

	// If the extension is launched in debug mode then the debug server options are used
	// Otherwise the run options are used
	let serverOptions: ServerOptions = {
		// run : { command: serverModule, args: [arg] },
		run: { command: runCommand },
		// debug: { command: serverModule, args: [arg], options:{detached: true} }
		debug: { command: runCommand, options: { detached: true } }
	}

	// Options to control the language client
	let clientOptions: LanguageClientOptions = {
		// Register the server for plain text documents
		documentSelector: [{ scheme: 'file', language: 'fryscript' }],
		synchronize: {
			// Synchronize the setting section 'languageServerExample' to the server
			configurationSection: 'lspSample',
			// Notify the server about file changes to '.clientrc files contain in the workspace
			fileEvents: workspace.createFileSystemWatcher('**/.clientrc')
		}
	}

	// Create the language client and start the client.
	let disposable = new LanguageClient('lspSample', 'Language Server Example', serverOptions, clientOptions).start();

	// Push the disposable to the context's subscriptions so that the 
	// client can be deactivated on extension deactivation
	context.subscriptions.push(disposable);
}
