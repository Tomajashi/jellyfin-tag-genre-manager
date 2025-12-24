// Assuming this is the structure of your plugin

let presets = { tags: [], genres: [] };
let currentAssignment = { tags: [], genres: [] };
let currentItemId = null;

// Get pluginId from URL
const urlParams = new URLSearchParams(window.location.search);
const pluginId = urlParams.get('pluginId') || 'TagGenreManager'; // fallback

async function apiGet(url) {
  return ApiClient.getJSON(ApiClient.getUrl("plugins/" + pluginId + "/" + url));
}

async function apiPost(url, body) {
  return ApiClient.ajax({
    type: "POST",
    url: ApiClient.getUrl("plugins/" + pluginId + "/" + url),
    data: JSON.stringify(body ?? {}),
    contentType: "application/json"
  });
}

// Render tags and genres in the UI
function renderPresets() {
  const tagsUl = document.getElementById("savedTags");
  const genresUl = document.getElementById("savedGenres");
  tagsUl.innerHTML = "";
  genresUl.innerHTML = "";

  presets.tags.forEach(tag => {
    const li = document.createElement("li");
    li.textContent = tag;
    tagsUl.appendChild(li);
  });

  presets.genres.forEach(genre => {
    const li = document.createElement("li");
    li.textContent = genre;
    genresUl.appendChild(li);
  });

  // Add tags and genres to the select dropdowns
  const tagSelect = document.getElementById("tagSelect");
  const genreSelect = document.getElementById("genreSelect");
  tagSelect.innerHTML = "";
  genreSelect.innerHTML = "";

  presets.tags.forEach(tag => {
    const option = document.createElement("option");
    option.value = tag;
    option.textContent = tag;
    tagSelect.appendChild(option);
  });

  presets.genres.forEach(genre => {
    const option = document.createElement("option");
    option.value = genre;
    option.textContent = genre;
    genreSelect.appendChild(option);
  });
}

// Fetch presets when the page loads
async function loadPresets() {
  presets = await apiGet("CustomMeta/presets");
  renderPresets();
}

// Fetch and display search results
async function searchMedia() {
  const term = document.getElementById("searchTerm").value;
  const results = await apiGet(`CustomMeta/search?term=${encodeURIComponent(term)}`);

  const searchResults = document.getElementById("searchResults");
  searchResults.innerHTML = "";

  results.forEach(item => {
    const li = document.createElement("li");
    li.textContent = item.name;
    const selectBtn = document.createElement("button");
    selectBtn.textContent = "Select";
    selectBtn.onclick = () => loadItem(item.id);
    li.appendChild(selectBtn);
    searchResults.appendChild(li);
  });
}

// Display item details and allow tagging
async function loadItem(itemId) {
  const item = await apiGet(`CustomMeta/item/${itemId}`);
  document.getElementById("itemTitle").textContent = item.name;
  currentItemId = itemId;
  currentAssignment = { tags: item.tags, genres: item.genres };

  // Render tags/genres of the item
  const itemTags = document.getElementById("itemTags");
  const itemGenres = document.getElementById("itemGenres");
  itemTags.innerHTML = "";
  itemGenres.innerHTML = "";

  currentAssignment.tags.forEach(tag => {
    const li = document.createElement("li");
    li.textContent = tag;
    itemTags.appendChild(li);
  });

  currentAssignment.genres.forEach(genre => {
    const li = document.createElement("li");
    li.textContent = genre;
    itemGenres.appendChild(li);
  });

  // Show the item panel
  document.getElementById("itemPanel").style.display = "block";
}

// Add a tag to the item
document.getElementById("addTagToItemBtn").onclick = () => {
  const tag = document.getElementById("tagSelect").value;
  if (!currentAssignment.tags.includes(tag)) {
    currentAssignment.tags.push(tag);
    renderItemAssignment();
  }
};

// Add a genre to the item
document.getElementById("addGenreToItemBtn").onclick = () => {
  const genre = document.getElementById("genreSelect").value;
  if (!currentAssignment.genres.includes(genre)) {
    currentAssignment.genres.push(genre);
    renderItemAssignment();
  }
};

// Render current item tags and genres
function renderItemAssignment() {
  const itemTags = document.getElementById("itemTags");
  const itemGenres = document.getElementById("itemGenres");
  itemTags.innerHTML = "";
  itemGenres.innerHTML = "";

  currentAssignment.tags.forEach(tag => {
    const li = document.createElement("li");
    li.textContent = tag;
    itemTags.appendChild(li);
  });

  currentAssignment.genres.forEach(genre => {
    const li = document.createElement("li");
    li.textContent = genre;
    itemGenres.appendChild(li);
  });
}

// Save the changes and apply them to the item
document.getElementById("saveApplyBtn").onclick = async () => {
  if (!currentItemId) return;
  await apiPost(`CustomMeta/item/${currentItemId}`, currentAssignment);
  alert("Tags and genres applied!");
};

// Load presets on page load
loadPresets();

// Handle search button click
document.getElementById("searchBtn").onclick = searchMedia;

// Handle add tag button
document.getElementById("addTagBtn").onclick = async () => {
  const newTag = document.getElementById("newTag").value.trim();
  if (newTag) {
    await apiPost("CustomMeta/presets/tags", { Value: newTag });
    document.getElementById("newTag").value = "";
    await loadPresets();
  }
};

// Handle add genre button
document.getElementById("addGenreBtn").onclick = async () => {
  const newGenre = document.getElementById("newGenre").value.trim();
  if (newGenre) {
    await apiPost("CustomMeta/presets/genres", { Value: newGenre });
    document.getElementById("newGenre").value = "";
    await loadPresets();
  }
};
