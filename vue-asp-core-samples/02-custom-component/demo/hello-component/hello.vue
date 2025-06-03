<template>
  <div>
    Hello from custom component!<br />
    <strong>ID:</strong> {{ id }}
  </div>
</template>

<script>
export default {
  name: "hello",
  props: {
    id: { type: String, required: true },
  },
};
</script>
