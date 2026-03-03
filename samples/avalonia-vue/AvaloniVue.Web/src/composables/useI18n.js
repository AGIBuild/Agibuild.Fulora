import { ref } from 'vue';
import { getTranslations } from '@/i18n/translations';
const locale = ref('en');
export function setI18nLocale(l) {
    locale.value = l;
    document.documentElement.lang = l;
}
export function useI18n() {
    const t = (key, params) => {
        const dict = getTranslations(locale.value);
        let text = dict[key] ?? key;
        if (params) {
            for (const [k, v] of Object.entries(params)) {
                text = text.replace(`{${k}}`, String(v));
            }
        }
        return text;
    };
    return {
        locale: locale,
        setLocale: setI18nLocale,
        t,
    };
}
