/**
 * audioInterop.js
 * Web Audio API beep — called via Blazor JS interop.
 * Uses a shared AudioContext to avoid hitting the browser's context limit.
 */
(function () {
    let _ctx = null;

    function getContext() {
        if (!_ctx || _ctx.state === 'closed') {
            const Ctor = window.AudioContext || window.webkitAudioContext;
            if (!Ctor) return null;
            _ctx = new Ctor();
        }
        return _ctx;
    }

    function doBeep(ctx, frequency, duration) {
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();

        osc.connect(gain);
        gain.connect(ctx.destination);

        osc.type = 'sine';
        osc.frequency.setValueAtTime(frequency, ctx.currentTime);

        gain.gain.setValueAtTime(0.30, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + duration);

        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + duration);
    }

    window.playBeep = function (frequency, duration) {
        try {
            const ctx = getContext();
            if (!ctx) return;

            if (ctx.state === 'suspended') {
                ctx.resume().then(() => doBeep(ctx, frequency || 880, duration || 0.45));
            } else {
                doBeep(ctx, frequency || 880, duration || 0.45);
            }
        } catch (e) {
            console.warn('[MealTimer] Audio playback failed:', e);
        }
    };
}());
