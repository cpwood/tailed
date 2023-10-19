import chalk from 'chalk';
import * as readline from 'node:readline/promises';
import { ConsoleSession } from './ConsoleSession.js';

const session = new ConsoleSession();
await session.start(); // Anything after this will appear in Tailed.

const rl = readline.createInterface(process.stdin, process.stdout);
const name = await rl.question('What\'s your name? ');
rl.close();

console.log(`Hi ${name}!`);
console.clear();
console.log('A');
console.log('B');
console.log('C');
console.info('Info');
console.warn('Warn');
console.error('Error');
process.stderr.write('this is an error\n');
process.stderr.write(`this ${chalk.green('is an')} error\n`);

session.stop(); // Anything after this won't appear in Tailed.
console.log('Done!'); 