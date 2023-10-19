import chalk from 'chalk';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import * as readline from 'node:readline/promises';
import qrcode from 'qrcode-terminal';
import ShortUUID from 'shortuuid';

export class ConsoleSession {
    _stdOutWrite = process.stdout.write;
    _stdErrWrite = process.stderr.write;
    _consoleInfo = console.info;
    _consoleWarn = console.warn;
    _consoleError = console.error;
    _sessionId = new ShortUUID().uuid();
    _connection;
    _hostname;
    _url;

    get url() {
        return this._url;
    }

    async start(hostname = 'tailed.live') {
        this._hostname = hostname;
        this._url = `https://${hostname}/${this._sessionId}`;

        this._connection = new HubConnectionBuilder()
            .withUrl(`https://${hostname}/api/tail`)
            .configureLogging(LogLevel.Warning)
            .build();

        await this._connection.start();

        await this._renderSessionInfo();
        this._patch();
    }

    stop() {
        this._unpatch();
        
        // Since we haven't awaited any previous invokes, send one last one
        // and, when its promise has completed, stop the connection. This does
        // need to be done in this manner and not as an async method with awaits.
        this._connection.invoke('NoOp').then(() => {
            this._connection.stop();
        });
    }

    async _renderSessionInfo() {
        const rl = readline.createInterface(process.stdin, process.stdout);
        console.clear();

        qrcode.generate(this._url, {small: true}, (qrcode) => {
            console.log(this._url);
            console.log();
            console.log(qrcode);
        });

        await rl.question('Press the enter key to continue..');
        rl.close();

        console.clear();
    }

    _patch() {
        process.stdout.write = (args) => {
            args = Array.isArray(args) ? args : [ args ];
            this._stdOutWrite.apply(process.stdout, args);
            this._connection.invoke('SendData', this._sessionId, args[0]);
        }
        
        process.stderr.write = (args) => {
            args = Array.isArray(args) ? args : [ args ];
        
            if (args[0].indexOf('\x1b') != 0)
                args[0] = chalk.red(args[0]);
            
            this._stdErrWrite.apply(process.stderr, args);
            this._connection.invoke('SendData', this._sessionId, args[0]);
        }
        
        console.info = (args) => {
            args = Array.isArray(args) ? args : [ args ];
            args[0] = chalk.green(args[0]);
            this._consoleInfo.apply(console, args);
        }
        
        console.warn = (args) => {
            args = Array.isArray(args) ? args : [ args ];
            args[0] = chalk.yellow(args[0]);
            this._consoleWarn.apply(console, args);
        }
        
        console.error = (args) => {
            args = Array.isArray(args) ? args : [ args ];
            args[0] = chalk.red(args[0]);
            this._consoleError.apply(console, args);
        }
    }

    _unpatch() {
        process.stdout.write = this._stdOutWrite;
        process.stderr.write = this._stdErrWrite;
        console.info = this._consoleInfo;
        console.warn = this._consoleWarn;
        console.error = this._consoleError;
    }
}