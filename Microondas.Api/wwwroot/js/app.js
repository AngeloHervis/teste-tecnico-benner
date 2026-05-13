import * as api from './api.js';

let token = sessionStorage.getItem('jwt');
let currentInput = "";
let currentPower = null;
let eventSource = null;

function showError(msg) {
    const el = document.getElementById('display-alert');
    el.innerText = msg;
    setTimeout(() => {
        el.innerText = '';
    }, 5000);
}

function showSuccess(msg) {
    const el = document.getElementById('display-info');
    const original = el.innerText;
    el.innerText = msg;
    el.style.color = "var(--primary)";
    setTimeout(() => {
        el.innerText = original;
        el.style.color = "";
    }, 3000);
}

globalThis.onload = () => {
    updateAuthUI();
    if (token) {
        loadPrograms();
        startStreaming();
    }

    document.addEventListener('keydown', (e) => {
        if (!token) return;

        if (e.key >= '0' && e.key <= '9') {
            appendNum(e.key);
        } else if (e.key === 'Enter') {
            startMicrowave();
        } else if (e.key === 'Backspace' || e.key === 'Escape') {
            stopMicrowave();
        } else if (e.key.toLowerCase() === 'p') {
            setPower();
        }
    });
}

globalThis.login = async () => {
    const user = document.getElementById('username').value;
    const pass = document.getElementById('password').value;

    try {
        const data = await api.login(user, pass);
        if (data.token) {
            token = data.token;
            sessionStorage.setItem('jwt', token);
            updateAuthUI();
            await loadPrograms();
            startStreaming();
        } else {
            showError('Usuário ou senha inválidos');
        }
    } catch (err) {
        showError('Erro de conexão com servidor');
    }
}

globalThis.logout = () => {
    token = null;
    sessionStorage.removeItem('jwt');
    if (eventSource) eventSource.close();
    updateAuthUI();
    document.getElementById('program-list').innerHTML = '';
}

function updateAuthUI() {
    const indicator = document.getElementById('auth-indicator');
    const authText = document.getElementById('auth-text');
    const loginForm = document.getElementById('login-form');
    const authActions = document.getElementById('auth-actions');
    const appContainer = document.getElementById('app-container');

    if (token) {
        indicator.classList.add('authenticated');
        authText.innerText = "Autenticado";
        loginForm.style.display = 'none';
        authActions.style.display = 'flex';
        appContainer.classList.add('active');
    } else {
        indicator.classList.remove('authenticated');
        authText.innerText = "Não Autenticado";
        loginForm.style.display = 'flex';
        authActions.style.display = 'none';
        appContainer.classList.remove('active');
    }
}

globalThis.appendNum = (num) => {
    document.getElementById('display-alert').innerText = '';

    if (currentInput.length < 4) {
        currentInput += num;
        updateDisplayLocal();
    }
}

globalThis.clearInput = () => {
    const info = document.getElementById('display-info').innerText;
    if (info !== 'Inativo' && info !== 'Pronto' && info !== 'Concluido') return;

    document.getElementById('display-alert').innerText = '';
    currentInput = "";
    currentPower = null;
    updateDisplayLocal();
}

globalThis.setPower = () => {
    const defaultPower = currentPower || 10;
    const p = prompt("Digite a potência (1 a 10):", defaultPower.toString());
    if (p) {
        let num = Number.parseInt(p);
        if (!Number.isNaN(num)) {
            num = Math.max(1, Math.min(10, num));
            currentPower = num;
            document.getElementById('display-power').innerText = `Potência: ${num}`;
        }
    }
}

function updateDisplayLocal() {
    const display = document.getElementById('display-time');
    let padded = currentInput.padStart(4, '0');
    display.innerText = `${padded.substring(0, 2)}:${padded.substring(2, 4)}`;
}

async function loadPrograms() {
    try {
        const programs = await api.fetchPrograms(token);
        const list = document.getElementById('program-list');
        list.innerHTML = '';
        programs.forEach(p => {
            const item = document.createElement('div');
            item.className = 'program-item';
            const nameClass = p.ehPadrao ? '' : 'program-custom';
            item.innerHTML = `
                <div class="program-title ${nameClass}">${p.nome}</div>
                <div class="program-details">
                    <span>${p.alimento}</span>
                    <span>•</span>
                    <span>${p.tempoSegundos}s</span>
                    <span>•</span>
                    <span>Pot: ${p.potencia}</span>
                </div>
            `;
            item.onclick = () => api.startUiProgramHeating(token, p.id);
            list.appendChild(item);
        });
    } catch (e) {
        showError('Falha ao carregar programas: ' + e.message);
    }
}

function startStreaming() {
    if (eventSource) eventSource.close();
    eventSource = api.subscribeToStream((data) => {
        let timeToShow = data.TempoFormatado;
        if (timeToShow.endsWith('s')) {
            timeToShow = `00:${timeToShow.replace('s', '').padStart(2, '0')}`;
        } else if (timeToShow.includes(':')) {
            const parts = timeToShow.split(':');
            timeToShow = `${parts[0].padStart(2, '0')}:${parts[1]}`;
        }

        if (data.Estado !== 'Inativo' || currentInput === "") {
            document.getElementById('display-time').innerText = timeToShow;
            let statusText = data.Estado;
            if (data.Estado === 'EmAndamento') statusText = 'Em Andamento';
            if (data.Estado === 'Inativo') statusText = 'Pronto';

            document.getElementById('display-info').innerText = statusText;
        }

        document.getElementById('heating-string').innerText = data.VisorAquecimento;

        if (data.Estado === 'Concluido') {
            currentInput = "";
        }
    }, token);
}


globalThis.startMicrowave = async () => {
    try {
        if (currentInput === "") {
            await api.startUiQuickHeating(token);
        } else {
            let padded = currentInput.padStart(4, '0');
            let totalSecs = Number.parseInt(padded.substring(0, 2)) * 60 + Number.parseInt(padded.substring(2, 4));

            await api.startUiHeating(token, {
                tempoEmSegundos: totalSecs,
                potencia: currentPower || 10
            });

            currentInput = "";
        }
    } catch (e) {
        showError(e.message || "Erro ao iniciar");
    }
}

globalThis.quickStart = async () => {
    try {
        await api.startUiQuickHeating(token);
        currentInput = "";
    } catch (e) {
        showError(e.message || "Erro no início rápido");
    }
}

globalThis.stopMicrowave = async () => {
    const infoEl = document.getElementById('display-info');
    const info = infoEl.innerText.trim();

    if (info.toUpperCase() !== 'EM ANDAMENTO') {
        currentInput = "";
        currentPower = null;
        document.getElementById('display-time').innerText = "00:00";
        document.getElementById('display-power').innerText = "Potência: 10";
        document.getElementById('display-info').innerText = "Pronto";
        document.getElementById('display-alert').innerText = "";
    }

    await api.stopUiHeating(token);
}

globalThis.abrirModalCadastro = () => {
    document.getElementById('modal-cadastro').classList.add('active');
}

globalThis.fecharModalCadastro = () => {
    document.getElementById('modal-cadastro').classList.remove('active');
}

globalThis.abrirModalConfig = () => {
    document.getElementById('modal-config').classList.add('active');
}

globalThis.fecharModalConfig = () => {
    document.getElementById('modal-config').classList.remove('active');
    document.getElementById('new-password').value = "";
}

globalThis.salvarConfig = async () => {
    const novaSenha = document.getElementById('new-password').value;
    if (!novaSenha) {
        showError("Digite a nova senha");
        return;
    }

    try {
        await api.updateCredentials(token, novaSenha);
        fecharModalConfig();
        showSuccess("Senha atualizada!");
        setTimeout(() => logout(), 2000);
    } catch (e) {
        showError(e.message || 'Falha ao atualizar');
    }
}

globalThis.salvarNovoPrograma = async () => {
    const nome = document.getElementById('prog-nome').value;
    const alimento = document.getElementById('prog-alimento').value;
    const tempo = Number.parseInt(document.getElementById('prog-tempo').value);
    const potencia = Number.parseInt(document.getElementById('prog-potencia').value);
    const caractere = document.getElementById('prog-caractere').value;
    const instrucoes = document.getElementById('prog-instrucoes').value;

    if (!nome || !alimento || Number.isNaN(tempo) || Number.isNaN(potencia) || !caractere) {
        showError("Preencha os campos obrigatórios");
        return;
    }

    const potenciaEfetiva = Math.max(1, Math.min(10, potencia));

    const prog = {
        nome,
        alimento,
        tempoSegundos: tempo,
        potencia: potenciaEfetiva,
        caractereAquecimento: caractere.charAt(0),
        instrucoes
    };

    try {
        await api.createProgram(token, prog);
        fecharModalCadastro();
        await loadPrograms();
        showSuccess("Programa salvo!");
    } catch (e) {
        showError(e.message || 'Falha ao salvar');
    }
}
