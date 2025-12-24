let selectedItemId = null;
let currentAssignment = { tags: [], genres: [] };
let presets = { tags: [], genres: [] };

async function apiGet(url) {
  return ApiClient.getJSON(ApiClient.getUrl(url));
}
async function apiPost(url, body) {
  return ApiClient.ajax({
    type: "POST",
    url: ApiClient.getUrl(url),
    data: JSON.stringify(body ?? {}),
    contentType: "application/json"
  });
}

function renderPresets() {
  // lists
  const tagsUl = document.getElementById("savedTags");
  const genresUl = document.getElementById("savedGenres");
  tagsUl.innerHTML = "";
  genresUl.innerHTML = "";

  presets.tags.forEach(t => {
    const li = document.createElement("li");
    li.textContent = t;
    tagsUl.appendChild(li);
  });
  presets.genres.forEach(g => {
    const li = document.createElement("li");
    li.textContent = g;
    genresUl.appendChild(li);
  });

  // dropdowns
  const tagSelect = document.getElementById("tagSelect");
  const genreSelect = document.getElementById("genreSelect");
  tagSelect.innerHTML = "";
  genreSelect.innerHTML = "";

  presets.tags.forEach(t => {
    const o = document.createElement("option");
    o.value = t; o.textContent = t;
    tagSelect.appendChild(o);
  });
  presets.genres.forEach(g => {
    const o = document.createElement("option");
    o.value = g; o.textContent = g;
    genreSelect.appendChild(o);
  });
}

function renderItemAssignment() {
  const tagsUl = document.getElementById("itemTags");
  const genresUl = document.getElementById("itemGenres");
  tagsUl.innerHTML = "";
  genresUl.innerHTML = "";

  currentAssignment.tags.forEach((t, idx) => {
    const li = document.createElement("li");
    const btn = document.createElement("button");
    btn.textContent = "x";
    btn.onclick = () => { currentAssignment.tags.splice(idx, 1); renderItemAssignment(); };
    li.textContent = t + " ";
    li.appendChild(btn);
    tagsUl.appendChild(li);
  });

  currentAssignment.genres.forEach((g, idx) => {
    const li = document.createElement("li");
    const btn = document.createElement("button");
    btn.textContent = "x";
    btn.onclick = () => { currentAssignment.genres.splice(idx, 1); renderItemAssignment(); };
    li.textContent = g + " ";
    li.appendChild(btn);
    genresUl.appendChild(li);
  });
}

async function loadPresets() {
  presets = await apiGet("/CustomMeta/presets");
  renderPresets();
}

async function loadItem(itemId) {
  selectedItemId = itemId;
  const data = await apiGet(`/CustomMeta/item/${itemId}`);
  document.getElementById("itemTitle").textContent = data.name;
  currentAssignment = { tags: data.tags ?? [], genres: data.genres ?? [] };
  document.getElementById("itemPanel").style.display = "block";
  renderItemAssignment();
}

document.getElementById("addTagBtn").onclick = async () => {
  const v = document.getElementById("newTag").value.trim();
  if (!v) return;
  await apiPost("/CustomMeta/presets/tags", { value: v });
  document.getElementById("newTag").value = "";
  await loadPresets();
};

document.getElementById("addGenreBtn").onclick = async () => {
  const v = document.getElementById("newGenre").value.trim();
  if (!v) return;
  await apiPost("/CustomMeta/presets/genres", { value: v });
  document.getElementById("newGenre").value = "";
  await loadPresets();
};

document.getElementById("searchBtn").onclick = async () => {
  const term = document.getElementById("searchTerm").value.trim();
  const results = await apiGet(`/CustomMeta/search?term=${encodeURIComponent(term)}`);

  const ul = document.getElementById("searchResults");
  ul.innerHTML = "";
  results.forEach(r => {
    const li = document.createElement("li");
    const btn = document.createElement("button");
    btn.textContent = "Select";
    btn.onclick = () => loadItem(r.id);
    li.textContent = `${r.name} (${r.type}) `;
    li.appendChild(btn);
    ul.appendChild(li);
  });
};

document.getElementById("addTagToItemBtn").onclick = () => {
  const v = document.getElementById("tagSelect").value;
  if (v && !currentAssignment.tags.includes(v)) currentAssignment.tags.push(v);
  renderItemAssignment();
};

document.getElementById("addGenreToItemBtn").onclick = () => {
  const v = document.getElementById("genreSelect").value;
  if (v && !currentAssignment.genres.includes(v)) currentAssignment.genres.push(v);
  renderItemAssignment();
};

document.getElementById("saveApplyBtn").onclick = async () => {
  if (!selectedItemId) return;
  await apiPost(`/CustomMeta/item/${selectedItemId}`, currentAssignment);
  Dashboard.alert("Saved + applied!");
};

loadPresets();
