<script lang="ts">
    import type { ConsoleLine } from './ConsoleLine';
    import { AnsiUp } from 'ansi_up';
    export let line: ConsoleLine;
    export let ansiUp: AnsiUp;
</script>

<style type="text/css">
</style>
<span id={`span_${line.id}`}>{@html ansiUp.ansi_to_html(line.content)}</span>