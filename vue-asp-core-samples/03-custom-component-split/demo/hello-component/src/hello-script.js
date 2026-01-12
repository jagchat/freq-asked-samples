export default {
  name: "hello",
  props: {
    id: { type: String, required: true },
  },
  data() {
    return {
      message: "",
    };
  },
  methods: {
    showMessage() {
      this.message = `Button clicked on component with ID: ${this.id}`;
    },
  },
};
