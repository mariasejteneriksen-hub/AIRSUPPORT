// Tynd wrapper omkring Chart.js, kaldt fra Blazor via JS-interop.
// Holder styr på instanser pr. canvas-id, så vi kan gendanne grafer ved re-render.
window.chartHelpers = (function () {
    const instances = {};

    function destroy(canvasId) {
        if (instances[canvasId]) {
            instances[canvasId].destroy();
            delete instances[canvasId];
        }
    }

    const gridColor = "#e5e5e5";
    const textColor = "#52514e";

    function renderBar(canvasId, labels, values, seriesLabel, color) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        instances[canvasId] = new Chart(ctx, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [{
                    label: seriesLabel,
                    data: values,
                    backgroundColor: color || "#2a78d6",
                    borderRadius: 4,
                    maxBarThickness: 48
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: seriesLabel, color: textColor }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { color: textColor } },
                    y: { grid: { color: gridColor }, ticks: { color: textColor }, beginAtZero: true }
                }
            }
        });
    }

    function renderScatter(canvasId, points, xLabel, yLabel, color) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        instances[canvasId] = new Chart(ctx, {
            type: "scatter",
            data: {
                datasets: [{
                    label: `${yLabel} vs. ${xLabel}`,
                    data: points,
                    backgroundColor: color || "#2a78d6",
                    pointRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: `${yLabel} vs. ${xLabel}`, color: textColor }
                },
                scales: {
                    x: {
                        title: { display: true, text: xLabel, color: textColor },
                        grid: { color: gridColor },
                        ticks: { color: textColor }
                    },
                    y: {
                        title: { display: true, text: yLabel, color: textColor },
                        grid: { color: gridColor },
                        ticks: { color: textColor }
                    }
                }
            }
        });
    }

    // Kompakt scatter til en celle i et scatterplot-matrix (pairwise) — ingen titel/akse-tekst,
    // kun tynde ticks, så et helt grid af små grafer kan stå side om side.
    function renderMiniScatter(canvasId, points, color) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        instances[canvasId] = new Chart(ctx, {
            type: "scatter",
            data: {
                datasets: [{
                    data: points,
                    backgroundColor: color || "#2a78d6",
                    pointRadius: 2.5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: false,
                plugins: { legend: { display: false } },
                scales: {
                    x: { grid: { color: gridColor }, ticks: { color: textColor, maxTicksLimit: 4, font: { size: 9 } } },
                    y: { grid: { color: gridColor }, ticks: { color: textColor, maxTicksLimit: 4, font: { size: 9 } } }
                }
            }
        });
    }

    // Søjlediagram (histogram) til et grid af mange grafer — viser nu både x- og y-akse,
    // da cellerne er store nok til at bære det.
    function renderMiniBar(canvasId, labels, values, color) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        instances[canvasId] = new Chart(ctx, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: color || "#2a78d6"
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: false,
                plugins: { legend: { display: false } },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { color: textColor, maxTicksLimit: 6, font: { size: 11 }, maxRotation: 60, minRotation: 45 }
                    },
                    y: { grid: { color: gridColor }, ticks: { color: textColor, maxTicksLimit: 5, font: { size: 13 } }, beginAtZero: true }
                }
            }
        });
    }

    // Linjegraf — bruges til at vise en udvikling over tid (fx omsætning pr. år)
    function renderLine(canvasId, labels, values, seriesLabel, color) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        instances[canvasId] = new Chart(ctx, {
            type: "line",
            data: {
                labels: labels,
                datasets: [{
                    label: seriesLabel,
                    data: values,
                    borderColor: color || "#2a78d6",
                    backgroundColor: color || "#2a78d6",
                    pointRadius: 5,
                    borderWidth: 3,
                    tension: 0.15,
                    fill: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: seriesLabel, color: textColor, font: { size: 16 } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { color: textColor, font: { size: 13 } } },
                    y: { grid: { color: gridColor }, ticks: { color: textColor, font: { size: 13 } }, beginAtZero: true }
                }
            }
        });
    }

    // Søjlediagram til vækst-% år-til-år — grøn for positiv vækst, rød for negativ.
    // Værdier kan være null (fx første år, hvor der ikke findes et foregående år at sammenligne med).
    function renderGrowthBar(canvasId, labels, values, seriesLabel) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        const colors = values.map(v => (v === null || v === undefined) ? "#c3c2b7" : (v >= 0 ? "#1baf7a" : "#e34948"));

        instances[canvasId] = new Chart(ctx, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [{
                    label: seriesLabel,
                    data: values,
                    backgroundColor: colors,
                    borderRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: seriesLabel, color: textColor, font: { size: 16 } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { color: textColor, font: { size: 13 } } },
                    y: {
                        grid: { color: gridColor },
                        ticks: { color: textColor, font: { size: 13 }, callback: v => v + "%" }
                    }
                }
            }
        });
    }

    // Søjlediagram hvor hver søjle har sin egen farve — bruges til at sammenligne to grupper
    // (fx "Aktive" vs. "Churnede") på ét nøgletal ad gangen.
    function renderBarWithColors(canvasId, labels, values, colors, title) {
        destroy(canvasId);
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        instances[canvasId] = new Chart(ctx, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors,
                    borderRadius: 4,
                    maxBarThickness: 80
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: title, color: textColor, font: { size: 14 } }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { color: textColor, font: { size: 13 } } },
                    y: { grid: { color: gridColor }, ticks: { color: textColor, font: { size: 12 } }, beginAtZero: true }
                }
            }
        });
    }

    return { renderBar, renderScatter, renderMiniScatter, renderMiniBar, renderLine, renderGrowthBar, renderBarWithColors, destroy };
})();
