<template>
  <div>
    <h1>WEXO Depot</h1>
    
    <!-- Search Bar -->
    <form id="search">
      Search: <input name="query" v-model="searchQuery">
    </form>

    <!-- Table for displaying items -->
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Name</th>
          <th>Amount</th>
          <th>Location</th>
        </tr>
      </thead>
      <tbody>
        <!-- Loop over filteredItems to display rows -->
        <tr v-for="item in filteredItems" :key="item.id">
          <td>{{ item.id }}</td>
          <td>{{ item.navn }}</td>
          <td>{{ item.antal }}</td>
          <td>{{ item.lokation }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';

// Declare a state variable to store the fetched items
const items = ref([]);

// Declare a search query for filtering
const searchQuery = ref('');

// Fetch the items when the component is created
const fetchItems = async () => {
  try {
    // Fetch data from the correct API endpoint (ensure you're using HTTPS)
    const response = await axios.get("https://localhost:7092/api/Backend");

    // Log the raw response
    console.log('Raw response:', response);

    // Assuming the response data is an array of items
    items.value = response.data;  // Store the fetched data in 'items'

    // Log the parsed items to confirm the structure
    console.log('Fetched items:', items.value);
  } catch (error) {
    console.error("Error fetching items:", error);
  }
};

// Call the fetchItems function when the component is created
onMounted(() => {
  fetchItems();
});

// Create a computed property to filter the items based on the search query
const filteredItems = computed(() => {
  return items.value.filter(item => {
    return item.navn && item.navn.toLowerCase().includes(searchQuery.value.toLowerCase());
  });
});
</script>

<style scoped>
/* Basic styling for the table */
table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
  background-color: #fff; /* Ensure table has a white background */
}

th, td {
  padding: 10px;
  text-align: left;
  border: 1px solid #ddd;
}

th {
  background-color: #f4f4f4;
  color: #000; /* Make sure header text is black */
}

td {
  color: #000; /* Make sure table cell text is black */
}

tr:hover {
  background-color: #f9f9f9;
}
</style>
