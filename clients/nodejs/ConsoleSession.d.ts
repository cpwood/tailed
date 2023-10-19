/**
 * Creates and manages a Tailed session.
 */
export class ConsoleSession {
    /**
     * The browser URL where streamed content can be viewed.
     */
    url: string;

    /**
     * Starts the session and ensures that `console`, `process.stdout` and 
     * `process.stderr` are streamed to the Tailed server. Any such call that follows 
     * this command *will* be streamed to the Internet.
     * 
     * `console.info`, `console.warn` and `console.error` are rendered in color.
     * 
     * This method returns a Promise which the calling code should `await` or be 
     * followed with a `then()`. Failure to do so will result in unreliable
     * behavior.
     * @param hostname Optional override of the `tailed.live` hostname. Can be used for privately hosted servers.
     */
    start(hostname: string | undefined) : Promise;

    /**
     * Stops the streaming of content to the server. This must be called to
     * allow Node to terminate cleanly. Anything that follows 
     * this command will *not* be streamed to the Internet.
     */
    stop();
}