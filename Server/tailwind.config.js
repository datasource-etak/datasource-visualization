/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: "class",
    content: ["./**/*.{razor,html,cshtml}"],
    theme: {
        extend: {
            colors: {
                primary: {
                    "50": "#eef2ff",
                    "100": "#e0e7ff",
                    "200": "#c7d2fe",
                    "300": "#a5b4fc",
                    "400": "#818cf8",
                    "500": "#6366f1",
                    "600": "#4f46e5",
                    "700": "#4338ca",
                    "800": "#3730a3",
                    "900": "#312e81"
                },
                secondary: {
                    "50": "#f9fafb",
                    "100": "#f3f4f6",
                    "200": "#e5e7eb",
                    "300": "#d1d5db",
                    "400": "#9ca3af",
                    "500": "#6b7280",
                    "600": "#4b5563",
                    "700": "#374151",
                    "800": "#1f2937",
                    "900": "#111827"
                },
                error: {
                    "50": "#FEF2F2",
                    "100": "#FEE2E2",
                    "200": "#FECACA",
                    "300": "#FCA5A5",
                    "400": "#F87171",
                    "500": "#EF4444",
                    "600": "#DC2626",
                    "700": "#B91C1C",
                    "800": "#991B1B",
                    "900": "#7F1D1D"
                },
                success: {
                    "50": "#ecfdf5",
                    "100": "#d1fae5",
                    "200": "#a7f3d0",
                    "300": "#6ee7b7",
                    "400": "#34d399",
                    "500": "#10b981",
                    "600": "#059669",
                    "700": "#047857",
                    "800": "#065f46",
                    "900": "#064e3b"
                },
                warning: {
                    "50": "#fefce8",
                    "100": "#fef9c3",
                    "200": "#fef08a",
                    "300": "#fde047",
                    "400": "#facc15",
                    "500": "#eab308",
                    "600": "#ca8a04",
                    "700": "#a16207",
                    "800": "#854d0e",
                    "900": "#713f12"
                },
                info: {
                    "50": "#eff6ff",
                    "100": "#dbeafe",
                    "200": "#bfdbfe",
                    "300": "#93c5fd",
                    "400": "#60a5fa",
                    "500": "#3b82f6",
                    "600": "#2563eb",
                    "700": "#1d4ed8",
                    "800": "#1e40af",
                    "900": "#1e3a8a"
                }
            },
            fontFamily: {
                sans: ['Inter var',
                    "ui-sans-serif",
                    "system-ui",
                    "-apple-system",
                    "BlinkMacSystemFont",
                    "Segoe UI",
                    "Roboto",
                    "Helvetica Neue",
                    "Arial",
                    "Noto Sans",
                    "sans-serif",
                    "Apple Color Emoji",
                    "Segoe UI Emoji",
                    "Segoe UI Symbol",
                    "Noto Color Emoji"
                ],
            },
        },
    },
    corePlugins: {
        aspectRatio: false,
    },
    plugins: [
        require("@tailwindcss/typography"),
        require("@tailwindcss/forms"),
        require("@tailwindcss/aspect-ratio"),
        require("@tailwindcss/line-clamp"),
        require('@tailwindcss/container-queries')
    ],
}