window.vsGame = (() => {
    let ctx, canvas;
    const CYAN = '#00D9FF';
    const MAGENTA = '#FF006E';
    const BG = '#0a0e27';

    function init(canvasEl) {
        canvas = canvasEl;
        ctx = canvas.getContext('2d');
    }

    function focusContainer(el) {
        el.focus();
    }

    function render(state) {
        if (!ctx) return;
        const W = canvas.width, H = canvas.height;

        // Background
        ctx.fillStyle = BG;
        ctx.fillRect(0, 0, W, H);

        // Stars (static, seeded by position)
        drawStars(W, H);

        if (state.status === 1 || state.status === 2 || state.status === 4) { // Playing, Paused, LevelComplete
            drawAsteroids(state.asteroids);
            drawBullets(state.bullets);
            drawParticles(state.particles);
            if (state.player && state.player.isActive) drawPlayer(state.player);
            drawHUD(state);
        }

        if (state.status === 0) drawMenu(W, H);           // Menu
        if (state.status === 2) drawPaused(W, H);         // Paused
        if (state.status === 4) drawLevelComplete(state, W, H); // LevelComplete
        if (state.status === 3) drawGameOver(state, W, H); // GameOver
    }

    function drawStars(W, H) {
        ctx.fillStyle = 'rgba(255,255,255,0.6)';
        // deterministic pseudo-stars using pre-baked positions
        const stars = [[50,80],[200,30],[380,120],[600,50],[800,90],[150,300],[420,250],[700,320],[900,150],[100,500],[350,480],[650,520],[850,440],[250,620],[500,680],[750,600],[950,700],[80,700],[460,380],[820,560]];
        for (const [x,y] of stars) {
            ctx.beginPath();
            ctx.arc(x, y, 0.8, 0, Math.PI * 2);
            ctx.fill();
        }
    }

    function drawPlayer(player) {
        ctx.save();
        ctx.translate(player.x, player.y);
        ctx.rotate(player.rotation);

        // Thruster flame
        if (player.isThrusting) {
            const flameLen = 8 + Math.random() * 10;
            ctx.strokeStyle = MAGENTA;
            ctx.lineWidth = 1.5;
            ctx.shadowColor = MAGENTA;
            ctx.shadowBlur = 10;
            ctx.beginPath();
            ctx.moveTo(-5, 9);
            ctx.lineTo(0, 9 + flameLen);
            ctx.lineTo(5, 9);
            ctx.stroke();
            ctx.shadowBlur = 0;
        }

        // Ship body
        ctx.strokeStyle = CYAN;
        ctx.lineWidth = 2;
        ctx.shadowColor = CYAN;
        ctx.shadowBlur = 8;
        ctx.beginPath();
        ctx.moveTo(0, -12);
        ctx.lineTo(9, 10);
        ctx.lineTo(0, 6);
        ctx.lineTo(-9, 10);
        ctx.closePath();
        ctx.stroke();
        ctx.shadowBlur = 0;

        ctx.restore();
    }

    function drawAsteroids(asteroids) {
        for (const ast of asteroids) {
            if (!ast.isActive) continue;
            ctx.save();
            ctx.translate(ast.x, ast.y);
            ctx.rotate(ast.rotation);

            ctx.strokeStyle = CYAN;
            ctx.lineWidth = 1.5;
            ctx.shadowColor = CYAN;
            ctx.shadowBlur = 4;

            // Irregular polygon
            const sides = ast.size === 40 ? 10 : ast.size === 20 ? 8 : 6;
            const r = ast.radius;
            ctx.beginPath();
            for (let i = 0; i < sides; i++) {
                const angle = (i / sides) * Math.PI * 2;
                // Use size as seed for irregularity (deterministic)
                const jitter = 0.75 + ((i * ast.size * 7) % 31) / 100;
                const px = Math.cos(angle) * r * jitter;
                const py = Math.sin(angle) * r * jitter;
                i === 0 ? ctx.moveTo(px, py) : ctx.lineTo(px, py);
            }
            ctx.closePath();
            ctx.stroke();
            ctx.shadowBlur = 0;
            ctx.restore();
        }
    }

    function drawBullets(bullets) {
        for (const b of bullets) {
            if (!b.isActive) continue;
            ctx.save();
            ctx.fillStyle = CYAN;
            ctx.shadowColor = CYAN;
            ctx.shadowBlur = 12;
            ctx.beginPath();
            ctx.arc(b.x, b.y, b.radius, 0, Math.PI * 2);
            ctx.fill();
            ctx.shadowBlur = 0;
            ctx.restore();
        }
    }

    function drawParticles(particles) {
        for (const p of particles) {
            if (!p.isActive) continue;
            const alpha = p.maxLifetime > 0 ? p.lifetime / p.maxLifetime : 0;
            ctx.save();
            ctx.globalAlpha = alpha * 0.9;
            ctx.fillStyle = p.color || CYAN;
            ctx.shadowColor = p.color || CYAN;
            ctx.shadowBlur = 6;
            ctx.beginPath();
            ctx.arc(p.x, p.y, p.radius * (0.5 + alpha * 0.5), 0, Math.PI * 2);
            ctx.fill();
            ctx.shadowBlur = 0;
            ctx.restore();
        }
    }

    function drawHUD(state) {
        const pad = 20;
        ctx.font = 'bold 18px "Courier New"';
        ctx.fillStyle = CYAN;
        ctx.shadowColor = CYAN;
        ctx.shadowBlur = 8;
        ctx.textAlign = 'left';
        ctx.fillText(`SCORE  ${String(state.score).padStart(6, '0')}`, pad, pad + 18);
        ctx.textAlign = 'center';
        ctx.fillText(`LEVEL ${state.level}`, canvas.width / 2, pad + 18);
        ctx.textAlign = 'right';
        ctx.fillText(`FPS ${Math.round(state.fps)}`, canvas.width - pad, pad + 18);

        // Lives
        ctx.textAlign = 'right';
        ctx.font = '14px "Courier New"';
        ctx.fillStyle = MAGENTA;
        ctx.shadowColor = MAGENTA;
        const livesText = '▲ '.repeat(Math.max(0, state.player?.lives ?? 0));
        ctx.fillText(livesText, canvas.width - pad, pad + 42);
        ctx.shadowBlur = 0;
    }

    function drawMenu(W, H) {
        ctx.fillStyle = 'rgba(10, 14, 39, 0.75)';
        ctx.fillRect(0, 0, W, H);

        ctx.textAlign = 'center';
        ctx.shadowColor = CYAN;
        ctx.shadowBlur = 20;
        ctx.fillStyle = CYAN;
        ctx.font = 'bold 56px "Courier New"';
        ctx.fillText('VOID STRIKE', W / 2, H / 2 - 80);

        ctx.shadowBlur = 0;
        ctx.fillStyle = 'rgba(0,217,255,0.5)';
        ctx.font = '14px "Courier New"';
        ctx.fillText('─────────────────────────────────', W / 2, H / 2 - 40);

        ctx.fillStyle = '#aaa';
        ctx.font = '16px "Courier New"';
        ctx.fillText('W  ─  Thrust        A / D  ─  Rotate', W / 2, H / 2);
        ctx.fillText('SPACE  ─  Shoot          P  ─  Pause', W / 2, H / 2 + 28);

        ctx.shadowColor = CYAN;
        ctx.shadowBlur = 12;
        ctx.fillStyle = CYAN;
        ctx.font = 'bold 22px "Courier New"';
        ctx.fillText('[ PRESS SPACE TO START ]', W / 2, H / 2 + 90);
        ctx.shadowBlur = 0;
    }

    function drawPaused(W, H) {
        ctx.fillStyle = 'rgba(10, 14, 39, 0.6)';
        ctx.fillRect(0, 0, W, H);
        ctx.textAlign = 'center';
        ctx.shadowColor = CYAN;
        ctx.shadowBlur = 16;
        ctx.fillStyle = CYAN;
        ctx.font = 'bold 42px "Courier New"';
        ctx.fillText('PAUSED', W / 2, H / 2);
        ctx.shadowBlur = 0;
        ctx.fillStyle = '#aaa';
        ctx.font = '18px "Courier New"';
        ctx.fillText('Press P to resume', W / 2, H / 2 + 50);
    }

    function drawLevelComplete(state, W, H) {
        ctx.fillStyle = 'rgba(10, 14, 39, 0.55)';
        ctx.fillRect(0, 0, W, H);
        ctx.textAlign = 'center';
        ctx.shadowColor = CYAN;
        ctx.shadowBlur = 20;
        ctx.fillStyle = CYAN;
        ctx.font = 'bold 42px "Courier New"';
        ctx.fillText(`LEVEL ${state.level - 1} COMPLETE`, W / 2, H / 2 - 20);
        ctx.shadowBlur = 0;
        ctx.fillStyle = '#aaa';
        ctx.font = '18px "Courier New"';
        ctx.fillText('Preparing next wave…', W / 2, H / 2 + 30);
    }

    function drawGameOver(state, W, H) {
        ctx.fillStyle = 'rgba(10, 14, 39, 0.80)';
        ctx.fillRect(0, 0, W, H);
        ctx.textAlign = 'center';
        ctx.shadowColor = MAGENTA;
        ctx.shadowBlur = 24;
        ctx.fillStyle = MAGENTA;
        ctx.font = 'bold 56px "Courier New"';
        ctx.fillText('GAME OVER', W / 2, H / 2 - 60);
        ctx.shadowBlur = 0;
        ctx.fillStyle = CYAN;
        ctx.font = '24px "Courier New"';
        ctx.fillText(`SCORE  ${String(state.score).padStart(6, '0')}`, W / 2, H / 2);
        ctx.fillStyle = '#aaa';
        ctx.font = '18px "Courier New"';
        ctx.fillText('Press R to restart', W / 2, H / 2 + 60);
    }

    return { init, render, focusContainer };
})();
