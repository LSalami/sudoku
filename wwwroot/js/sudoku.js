function focusCell(row, col) {
    const cell = document.getElementById(`cell-${row}-${col}`);
    if (cell) {
        cell.focus();
    }
}

// Salvataggio puzzle in localStorage
function savePuzzle(key, name, gridJson, puzzleType) {
    const puzzles = JSON.parse(localStorage.getItem('savedPuzzles') || '{}');
    puzzles[key] = {
        name: name,
        grid: gridJson,
        type: puzzleType,
        date: new Date().toISOString()
    };
    localStorage.setItem('savedPuzzles', JSON.stringify(puzzles));
}

function loadPuzzle(key) {
    const puzzles = JSON.parse(localStorage.getItem('savedPuzzles') || '{}');
    return puzzles[key] ? puzzles[key].grid : null;
}

function getSavedPuzzlesList(puzzleType) {
    const puzzles = JSON.parse(localStorage.getItem('savedPuzzles') || '{}');
    const list = [];
    for (const key in puzzles) {
        if (puzzles[key].type === puzzleType) {
            list.push({
                key: key,
                name: puzzles[key].name,
                date: puzzles[key].date
            });
        }
    }
    // Ordina per data decrescente
    list.sort((a, b) => new Date(b.date) - new Date(a.date));
    return list;
}

function deletePuzzle(key) {
    const puzzles = JSON.parse(localStorage.getItem('savedPuzzles') || '{}');
    delete puzzles[key];
    localStorage.setItem('savedPuzzles', JSON.stringify(puzzles));
}

// Esporta puzzle come file JSON
function downloadPuzzle(filename, gridJson, puzzleType) {
    const data = {
        name: filename,
        type: puzzleType,
        grid: JSON.parse(gridJson),
        exportDate: new Date().toISOString()
    };
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename.replace(/[^a-z0-9]/gi, '_') + '.json';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

// Trigger per aprire il file picker
function triggerFileInput(inputId) {
    document.getElementById(inputId).click();
}

// Legge il file selezionato
async function readFileContent(inputId) {
    const input = document.getElementById(inputId);
    if (!input.files || input.files.length === 0) return null;

    const file = input.files[0];
    const text = await file.text();
    input.value = ''; // Reset per permettere di ricaricare lo stesso file
    return text;
}
