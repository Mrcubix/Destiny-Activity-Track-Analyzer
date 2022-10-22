class AnimatedAlign extends HTMLElement {

    constructor() {

        super();

        this.oldNodes = [];
        // this.lastElementChild is a lie
        this.lastActualChild = null;
    }

    // https://stackoverflow.com/a/68537066/14919482
    connectedCallback() {

        // Set up observer
        this.observer = new MutationObserver(this.onMutation);
    
        // Watch the Light DOM for child node changes
        this.observer.observe(this, {
          childList: true
        });

        this.onInit()
    }

    onInit() {
        
        const children = Array.from(this.children);

        const before = document.createElement("span");
        this.insertBefore(before, children[0]);
        
        for (let i = 0; i < children.length; i++) {
            const child = children[i];

            child.remove()
            this.appendChild(child);
            this.lastActualChild = child;
        }

        const after = document.createElement("span");
        this.appendChild(after);

        this.oldNodes = Array.from(this.childNodes);
    }

    onChildsAppend(childs) {
        
        for (let i = 0; i < childs.length; i++) {
            const child = childs[i];
            this.insertBefore(child, this.lastActualChild.nextElementSibling);
            this.lastActualChild = child;
        }

        this.oldNodes = Array.from(this.childNodes);
    }
    
    disconnectedCallback() {

        // remove observer if element is no longer connected to DOM
        this.observer.disconnect();
    }
      
    onMutation(mutations) {

        const added = [];
    
        // A `mutation` is passed for each new node
        for (const mutation of mutations) {
          // Could test for `mutation.type` here, but since we only have
          // set up one observer type it will always be `childList`
          added.push(...mutation.addedNodes);
        }

        const parent = added[0].parentElement;

        let contained = 0;

        // We don't want to call onChildsAppend if the nodes are already present (init)
        // this probably has some underlying issues
        added.forEach(child => {
            console.log(child);
            // check if the child isn't already in the parentElement
            if (parent.oldNodes.includes(child)) {
                contained++;
            }
        })

        if (contained == added.length)
            return;

        parent.onChildsAppend(added);
    }
}

window.addEventListener('DOMContentLoaded', () => {

    customElements.define("animated-align", AnimatedAlign);
});