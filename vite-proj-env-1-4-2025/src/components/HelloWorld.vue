<template>
  <div>
    <h1>WEXO Depot</h1>
    <ul>
      <li v-for="item in items" :key="item.ID">
        {{ item.Navn }} - {{ item.Antal }} in {{ item.Lokation }}
      </li>
    </ul>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      items: [],  // This will store the fetched items
    };
  },
  async created() {
    try {
      // Fetch items from the correct API endpoint
      const response = await axios.get("http://localhost:7092/api/Backend"); // Corrected the URL
      this.items = response.data;  // Assign the fetched items to the 'items' data property
    } catch (error) {
      // Handle errors in the API call
      console.error("Error fetching items:", error);
    }
  },
};
</script>
