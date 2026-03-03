import { ref, watch } from 'vue';
import { appShellService } from '@/bridge/services';
/** Fetches the page list from C# AppShellService once bridge is ready. */
export function usePageRegistry(bridgeReady) {
    const pages = ref([]);
    const loading = ref(true);
    watch(bridgeReady, async (isReady) => {
        if (!isReady)
            return;
        try {
            pages.value = await appShellService.getPages();
        }
        catch (e) {
            console.error(e);
        }
        finally {
            loading.value = false;
        }
    }, { immediate: true });
    return { pages, loading };
}
