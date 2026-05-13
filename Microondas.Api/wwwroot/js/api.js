const API_URL = '/api';

export async function login(username, password) {
    const res = await fetch(`${API_URL}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    });

    if (!res.ok) {
        const errorData = await res.json();
        throw new Error(errorData.message || 'Erro no login');
    }

    const token = await res.text();
    return { token };
}

export async function fetchPrograms(token) {
    const res = await fetch(`${API_URL}/programas`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    return await res.json();
}

export async function createProgram(token, program) {
    const res = await fetch(`${API_URL}/programas`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(program)
    });

    const data = await res.json();

    if (!res.ok) {
        throw new Error(data.message || 'Falha ao salvar programa');
    }

    return data;
}

export async function startUiHeating(token, command) {
    const res = await fetch(`${API_URL}/aquecimento/iniciar`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(command)
    });
    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || 'Erro ao iniciar');
    }
}

export async function startUiQuickHeating(token) {
    const res = await fetch(`${API_URL}/aquecimento/rapido`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || 'Erro no início rápido');
    }
}

export async function startUiProgramHeating(token, id) {
    const res = await fetch(`${API_URL}/aquecimento/programa/${id}`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || 'Erro ao iniciar programa');
    }
}

export async function stopUiHeating(token) {
    const res = await fetch(`${API_URL}/aquecimento/pausar-cancelar`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || 'Erro ao pausar/cancelar');
    }
}

export async function updateCredentials(token, novaSenha) {
    const res = await fetch(`${API_URL}/auth/configurar`, {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify({ novaSenha })
    });
    
    if (!res.ok) {
        const error = await res.json();
        throw new Error(error.message || 'Erro ao atualizar credenciais');
    }
}

export function subscribeToStream(onUpdate, token) {
    const url = token 
        ? `${API_URL}/aquecimento/stream?access_token=${token}` 
        : `${API_URL}/aquecimento/stream`;
        
    const eventSource = new EventSource(url);
    eventSource.onmessage = (event) => {
        const data = JSON.parse(event.data);
        onUpdate(data);
    };
    return eventSource;
}
