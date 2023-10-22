<script lang="ts">
    import { AnsiUp } from 'ansi_up';
    import type { ConsoleLine } from './ConsoleLine';
    export let line: ConsoleLine;

    const ansiUp = new AnsiUp();
</script>

<style type="text/css">
</style>
<span id={`span_${line.id}`}>{@html ansiUp.ansi_to_html(line.content)}</span>