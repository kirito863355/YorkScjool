function addNebula(title, description) {
    const nebulaContainer = document.getElementById("nebulaContainer");
    const nebulaBox = document.createElement("div");
    nebulaBox.classList.add("nebula-box");
    nebulaBox.innerHTML = ` <img class="nebulaimg" src="./assets/images/favicon.png" alt=""> <div class="star-title">${title}</div><div class="cosmic-description">${description}</div>`;
    nebulaContainer.appendChild(nebulaBox);
}


const dictionaryDefaultEntries = [
    { "word": "Innovation", "definition": "The introduction of something new, especially a new idea, method, or device." },
    { "word": "Persistence", "definition": "The ability to continue doing something despite difficulties or opposition." },
    { "word": "Harmony", "definition": "A pleasing arrangement of parts, whether in music, relationships, or design." },
    { "word": "Integrity", "definition": "The quality of being honest and having strong moral principles." },
    { "word": "Resilience", "definition": "The capacity to recover quickly from difficulties; toughness." },
    { "word": "Ambition", "definition": "A strong desire to achieve something, typically requiring hard work and determination." },
    { "word": "Curiosity", "definition": "A strong desire to know or learn something." },
    { "word": "Optimism", "definition": "A tendency to expect the best possible outcome or dwell on the most hopeful aspects of a situation." },
    { "word": "Wisdom", "definition": "The ability to use knowledge and experience to make good decisions and judgments." },
    { "word": "Gratitude", "definition": "The quality of being thankful; readiness to show appreciation and return kindness." }
];

function loadDictionaryEntries() {
    let entries = JSON.parse(localStorage.getItem("dictionaryData"));
    if (!entries) {
        entries = dictionaryDefaultEntries;
        localStorage.setItem("dictionaryData", JSON.stringify(entries));
    }
    displayDictionaryEntries(entries);
}

function displayDictionaryEntries(entries) {
    let list = document.querySelector(".dictionary-word-list");
    list.innerHTML = "";
    entries.forEach((entry, index) => {
        let li = document.createElement("li");
        li.className = "dictionary-word-item";
        li.innerHTML = `<strong>${entry.word}:</strong> ${entry.definition} 
                        <button class="delete-word-btn" onclick="removeDictionaryEntry(${index})"><i><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#f32020" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-trash-2"><path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" x2="10" y1="11" y2="17"/><line x1="14" x2="14" y1="11" y2="17"/></svg></i></button>`;
        list.appendChild(li);
    });
}

function addDictionaryEntry() {
    let word = document.querySelector(".word-input-field").value.trim();
    let definition = document.querySelector(".definition-input-field").value.trim();
    if (word && definition) {
        let entries = JSON.parse(localStorage.getItem("dictionaryData")) || [];
        entries.push({ word, definition });
        localStorage.setItem("dictionaryData", JSON.stringify(entries));
        document.querySelector(".word-input-field").value = "";
        document.querySelector(".definition-input-field").value = "";
        displayDictionaryEntries(entries);
    }
}

function removeDictionaryEntry(index) {
    let entries = JSON.parse(localStorage.getItem("dictionaryData")) || [];
    entries.splice(index, 1);
    localStorage.setItem("dictionaryData", JSON.stringify(entries));
    displayDictionaryEntries(entries);
}

window.onload = loadDictionaryEntries;