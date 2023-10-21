<script lang="ts">
    import type { ConsoleLine } from './ConsoleLine';
    export let line: ConsoleLine;
</script>

<style type="text/css">
</style>
<span id={`span_${line.id}`}>{@html line.asHtml()}</span>