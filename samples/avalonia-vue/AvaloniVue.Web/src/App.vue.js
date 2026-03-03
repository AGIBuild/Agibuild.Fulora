import { watch, onMounted, onUnmounted } from 'vue';
import { useBridgeReady } from '@/composables/useBridge';
import { usePageRegistry } from '@/composables/usePageRegistry';
import { settingsService } from '@/bridge/services';
import { applySettings } from '@/utils/applySettings';
import { setI18nLocale } from '@/composables/useI18n';
import AppLayout from '@/components/AppLayout.vue';
const bridgeReady = useBridgeReady();
const { pages, loading } = usePageRegistry(bridgeReady);
watch(bridgeReady, async (isReady) => {
    if (!isReady)
        return;
    try {
        const s = await settingsService.getSettings();
        setI18nLocale(s.language);
        applySettings(s);
    }
    catch { /* ignore */ }
});
const onSettingsChanged = (e) => {
    const s = e.detail;
    if (s?.language)
        setI18nLocale(s.language);
};
onMounted(() => window.addEventListener('app-settings-changed', onSettingsChanged));
onUnmounted(() => window.removeEventListener('app-settings-changed', onSettingsChanged));
debugger; /* PartiallyEnd: #3632/scriptSetup.vue */
const __VLS_ctx = {};
let __VLS_components;
let __VLS_directives;
if (!__VLS_ctx.bridgeReady || __VLS_ctx.loading) {
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "flex items-center justify-center h-screen bg-gray-50 dark:bg-gray-950" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div, __VLS_intrinsicElements.div)({
        ...{ class: "text-center space-y-3" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.div)({
        ...{ class: "w-8 h-8 border-2 border-blue-500 border-t-transparent rounded-full animate-spin mx-auto" },
    });
    __VLS_asFunctionalElement(__VLS_intrinsicElements.p, __VLS_intrinsicElements.p)({
        ...{ class: "text-sm text-gray-500 dark:text-gray-400" },
    });
    (!__VLS_ctx.bridgeReady ? 'Connecting to bridge...' : 'Loading pages...');
}
else {
    /** @type {[typeof AppLayout, ]} */ ;
    // @ts-ignore
    const __VLS_0 = __VLS_asFunctionalComponent(AppLayout, new AppLayout({
        pages: (__VLS_ctx.pages),
    }));
    const __VLS_1 = __VLS_0({
        pages: (__VLS_ctx.pages),
    }, ...__VLS_functionalComponentArgsRest(__VLS_0));
    var __VLS_3 = {};
    var __VLS_2;
}
/** @type {__VLS_StyleScopedClasses['flex']} */ ;
/** @type {__VLS_StyleScopedClasses['items-center']} */ ;
/** @type {__VLS_StyleScopedClasses['justify-center']} */ ;
/** @type {__VLS_StyleScopedClasses['h-screen']} */ ;
/** @type {__VLS_StyleScopedClasses['bg-gray-50']} */ ;
/** @type {__VLS_StyleScopedClasses['dark:bg-gray-950']} */ ;
/** @type {__VLS_StyleScopedClasses['text-center']} */ ;
/** @type {__VLS_StyleScopedClasses['space-y-3']} */ ;
/** @type {__VLS_StyleScopedClasses['w-8']} */ ;
/** @type {__VLS_StyleScopedClasses['h-8']} */ ;
/** @type {__VLS_StyleScopedClasses['border-2']} */ ;
/** @type {__VLS_StyleScopedClasses['border-blue-500']} */ ;
/** @type {__VLS_StyleScopedClasses['border-t-transparent']} */ ;
/** @type {__VLS_StyleScopedClasses['rounded-full']} */ ;
/** @type {__VLS_StyleScopedClasses['animate-spin']} */ ;
/** @type {__VLS_StyleScopedClasses['mx-auto']} */ ;
/** @type {__VLS_StyleScopedClasses['text-sm']} */ ;
/** @type {__VLS_StyleScopedClasses['text-gray-500']} */ ;
/** @type {__VLS_StyleScopedClasses['dark:text-gray-400']} */ ;
var __VLS_dollars;
const __VLS_self = (await import('vue')).defineComponent({
    setup() {
        return {
            AppLayout: AppLayout,
            bridgeReady: bridgeReady,
            pages: pages,
            loading: loading,
        };
    },
});
export default (await import('vue')).defineComponent({
    setup() {
        return {};
    },
});
; /* PartiallyEnd: #4569/main.vue */
